from pytube import YouTube
import os

download_folder = "downloads"

if not os.path.exists(download_folder):
    os.mkdir(download_folder)

def download_video(url):
    try:
        yt = YouTube(url)
        print(f"Video Title: {yt.title}")
        print("Available Resolutions:")
        streams = yt.streams.filter(progressive=True, file_extension="mp4").all()
        for i, stream in enumerate(streams):
            print(f"[{i}] {stream.resolution}")
        choice = int(input("Select resolution index: "))
        print("Downloading...")
        streams[choice].download(output_path=download_folder)
        print(f"Downloaded to {download_folder}/{yt.title}.mp4")
    except Exception as e:
        print(f"Error: {e}")

url = input("Enter YouTube video URL: ")
download_video(url)
