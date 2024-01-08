# Tools.DiskPurger

dotnet tool that purges your local disk with a specific cutoff date.

NOTE: WIP

# Environment variables needed

| Variable name                           | Example value                                 | Description                                                                                                              |
| --------------------------------------- | --------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------ |
| CRON_EXPRESSION                         | `*/2 * * * *`                                 | Cron expression on when to trigger deletion process                                                                      |
| APP_THREADS                             | `100`                                         | Number of threads to delete files in.                                                                                    |
| GRACE_PERIOD_BEFORE_DELETING_IN_SECONDS | `15`                                          | Wait number of seconds before taking files into the deletion list (use this if you actively write to this local folder). |
| EXCLUDE_FILES                           | `/myfolder/myfile.ext,/myfolder/somefile.txt` | The files to exclude from uploading.                                                                                     |
| BASE_DIR                                | `/data`                                       | OPTIONAL: Path to the folder to upload. This defaults to `/data`                                                         |

# How to use

1. Make sure you filled all the above environment variables.
2. The data to be uploaded should be mounted at /data inside the container.

## Example

```
docker run --name=mydeleter -e CRON_EXPRESSION='*/2 * * * *' -e APP_THREADS=100 -e GRACE_PERIOD_BEFORE_DELETING_IN_SECONDS=15 -e EXCLUDE_FILES="/data/somefile.txt,/data/anotherfile.ext" -v /some/data/path/on/host:/data -d robhofmann/diskpurger
```

## Build it yourself

In the root of this repository:

```
docker build -f Docker/Dockerfile -t yourimagename .
```
