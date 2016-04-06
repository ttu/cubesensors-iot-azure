#### keys.py

keys.py is not in version control. Add it manually.

```py
db_user = "VALUE"
db_password = "VALUE!"
db_name = "VALUE"
db_server = "VALUE"

ifttt_api_key = "VALUE"
```

#### VirtualEnv, Tests & Cron

Check \src\python_parses\README.md

Execute hourly
```sh
SHELL=/bin/bash
0 * * * * cd /home/stetson/python_checker && source env/bin/activate && python checker.py
```