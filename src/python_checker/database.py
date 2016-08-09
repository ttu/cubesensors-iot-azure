import random
import string
import logging

import pymssql

import config

class DataStore(object):

    def __init__(self,  db_server, db_name, user_name, password):
        self.logger = logging.getLogger(config.logger_name)
        self.con_string = "Driver={SQL Server}; Server=tcp:" + db_server + ".database.windows.net,1433; Database=" + db_name + "; Uid=" + user_name + ";Pwd=" + password + ";"
        self.server = db_server + ".database.windows.net"
        self.db_name = db_name
        self.user_name = user_name + "@" + db_server
        self.password = password

    def getSensorBatteryStatuses(self):
        stmt = '''
        SELECT t1.SensorId, t1.Battery, t1.Cable FROM cubesensors_data t1
        JOIN (SELECT SensorId, MAX(MeasurementTime) MeasurementTime FROM cubesensors_data GROUP BY SensorId) t2
        ON t1.SensorId = t2.SensorId AND t1.MeasurementTime = t2.MeasurementTime
        WHERE ((t1.Battery < 10 AND t1.Cable = 0) OR (t1.Battery > 97 AND t1.Cable = 1))
        '''

        try:
            con = pymssql.connect(server=self.server, user=self.user_name, password=self.password, database = self.db_name)
            cursor = con.cursor()
            cursor.execute(stmt)
            rows = cursor.fetchall()
            cursor.close()
            con.close()
        except Exception as e:
            self.logger.error(e)
            return []

        return rows
