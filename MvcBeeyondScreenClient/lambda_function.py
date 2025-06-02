import os
import requests
import pymysql

# Variables de entorno
DB_HOST = os.environ['DB_HOST']
DB_NAME = os.environ['DB_NAME']
DB_USER = os.environ['DB_USER']
DB_PASS = os.environ['DB_PASS']
TMDB_BEARER = os.environ['TMDB_BEARER']

HEADERS = {
    "accept": "application/json",
    "Authorization": f"Bearer {TMDB_BEARER}"
}

URL_BASE_FONDO = "https://image.tmdb.org/t/p/w1066_and_h600_bestv2"
URL_BASE_POSTER = "https://image.tmdb.org/t/p/w440_and_h660_face"

def lambda_handler(event, context):
    url = "https://api.themoviedb.org/3/movie/now_playing?language=es-EU&page=1"
    response = requests.get(url, headers=HEADERS)

    if response.status_code != 200:
        return {
            'statusCode': response.status_code,
            'body': 'Error al obtener las películas de TMDb.'
        }

    data = response.json()
    peliculas = data.get("results", [])
    if not peliculas:
        return {'statusCode': 204, 'body': 'No se encontraron películas.'}

    try:
        conn = pymysql.connect(
            host=DB_HOST,
            user=DB_USER,
            password=DB_PASS,
            database=DB_NAME,
            cursorclass=pymysql.cursors.DictCursor
        )

        with conn:
            with conn.cursor() as cursor:
                # Obtener último PELICULA_ID + 1
                cursor.execute("SELECT COALESCE(MAX(PELICULA_ID), 0) + 1 AS next_id FROM PELICULA;")
                next_id_result = cursor.fetchone()
                next_id = next_id_result['next_id']

                for pelicula in peliculas:
                    tmdb_id = pelicula['id']
                    original_title = pelicula.get('original_title', '')
                    release_date = pelicula.get('release_date', None)
                    overview = pelicula.get('overview', '')
                    backdrop_path = pelicula.get('backdrop_path', '')
                    poster_path = pelicula.get('poster_path', '')

                    img_fondo = f"{URL_BASE_FONDO}{backdrop_path}" if backdrop_path else None
                    img_poster = f"{URL_BASE_POSTER}{poster_path}" if poster_path else None

                    # Llamada a detalles para obtener runtime y tagline
                    detalle_url = f"https://api.themoviedb.org/3/movie/{tmdb_id}?language=es-EU"
                    detalle_response = requests.get(detalle_url, headers=HEADERS)
                    detalle_json = detalle_response.json()

                    duracion = detalle_json.get('runtime', None)
                    titulo_etiqueta = detalle_json.get('tagline', '')

                    # Insertar en la BBDD
                    sql = """
                        INSERT INTO PELICULA (
                            PELICULA_ID, TITULO, FECHA_LANZAMIENTO, DURACION_MINUTOS,
                            TITULO_ETIQUETA, SINOPSIS, IMG_FONDO, IMG_POSTER, TMDB_ID
                        )
                        VALUES (%s, %s, %s, %s, %s, %s, %s, %s, %s)
                        ON DUPLICATE KEY UPDATE
                            TITULO = VALUES(TITULO),
                            FECHA_LANZAMIENTO = VALUES(FECHA_LANZAMIENTO),
                            DURACION_MINUTOS = VALUES(DURACION_MINUTOS),
                            TITULO_ETIQUETA = VALUES(TITULO_ETIQUETA),
                            SINOPSIS = VALUES(SINOPSIS),
                            IMG_FONDO = VALUES(IMG_FONDO),
                            IMG_POSTER = VALUES(IMG_POSTER);
                    """
                    cursor.execute(sql, (
                        next_id, original_title, release_date, duracion,
                        titulo_etiqueta, overview, img_fondo, img_poster, tmdb_id
                    ))

                    next_id += 1  # Incrementar ID para la siguiente película

            conn.commit()

        return {
            'statusCode': 200,
            'body': f'{len(peliculas)} películas insertadas o actualizadas correctamente.'
        }

    except Exception as e:
        print("Error:", e)
        return {
            'statusCode': 500,
            'body': f'Error al insertar en la base de datos: {str(e)}'
        }
