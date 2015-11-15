# Azure will execute first file.
# sys.path.append is required by Azure Web Jobs. It requires that all packages are provided to it in zip file.
# env\Lib\site-packages is virtual env path in Windows
import sys
sys.path.append("env\Lib\site-packages")

import logging
import logging.config

from datetime import datetime

import config

from database import DataStore
from ifttt import IFTTT

logging.config.fileConfig('log.config')
logger = logging.getLogger(config.logger_name)


def myExceptionHook(exctype, value, traceback):
    logger.error(value)
    sys.__excepthook__(exctype, value, traceback)
       
       
if __name__ == '__main__':
    sys.excepthook = myExceptionHook

    print("Running IFTTT checker at %s" % datetime.utcnow())
    
    store = DataStore(config.db_server, config.db_name, config.db_user, config.db_password)
    rows = store.getSensorBatteryStatuses()
    
    current_hour = datetime.utcnow().hour
    
    for row in rows:
        sensor_id = row[0]
        battery = row[1]
        cable = row[2]
  
        # If battery < 10 and cable 0 time to charge
        # But start charging only when utc time is over 22.00
        if battery < 10 and cable == 0: and current_hour > 21:
            logger.debug("Request charging %s (%s : %s)",  sensor_id, battery, cable)
            IFTTT.sendCharge(sensor_id)
        
        # Stop charging when nearing 100    
        if cable == 1 and battery > 98:
            logger.debug("Request unplug %s (%s : %s)",  sensor_id, battery, cable)
            IFTTT.sendStopCharge(sensor_id)
            
       
      