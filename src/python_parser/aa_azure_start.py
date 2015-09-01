# Azure will execute first file, so thats why this is named aa_XXXX.py
# sys.path.append is required by Azure Web Jobs. It requires that all packages are provided to it in zip file.
# env\Lib\site-packages is virtual env path in Windows
import sys
sys.path.append("env\Lib\site-packages")

import logging
import logging.config

import json
from datetime import datetime

import config

from azure_helper import EventHubHelper
from cube_parser import CubeParser

logging.config.fileConfig('log.config')
logger = logging.getLogger('cubesensors')


def myExceptionHook(exctype, value, traceback):
    logger.error(value)
    sys.__excepthook__(exctype, value, traceback)


if __name__ == '__main__':
    sys.excepthook = myExceptionHook

    print("Running at %s" % datetime.utcnow())
    datas = CubeParser().get_data()
    print("Latest data: %s" % datas[0]["time"])
    data_json = json.dumps(datas)
    EventHubHelper.send_request(config.servicebus_namespace, config.eventhub_name, config.eventhub_key_name, config.eventhub_private_key, data_json)
