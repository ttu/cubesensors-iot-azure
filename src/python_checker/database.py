import random
import string
import logging

import pypyodbc

import config

class DataStore(object):

	def __init__(self,  db_server, db_name, user_name, password):
		self.logger = logging.getLogger(config.logger_name)
		self.con_string = "Driver={SQL Server}; Server=tcp:" + db_server + ".database.windows.net,1433; Database=" + db_name + "; Uid=" + user_name + ";Pwd=" + password + ";"

	def getSensorBatteryStatuses(self):
		
		stmt = '''
		SELECT d1.SensorId, d1.Battery, d1.Cable
		FROM cubesensors_data d1 LEFT JOIN cubesensors_data d2
		ON (d1.SensorId = d2.SensorId AND d1.MeasurementTime < d2.MeasurementTime)
		WHERE d2.MeasurementTime IS NULL
		AND ((d1.Battery < 10 AND d1.Cable = 0) OR (d1.Battery > 97 AND d1.Cable = 1))
		AND d1.MeasurementTime > DATEADD(hour, -10, GETDATE())
		'''

		try:
			con = pypyodbc.connect(self.con_string)
			cursor = con.cursor()
			cursor.execute(stmt)

			rows = cursor.fetchall()

			cursor.commit()
			cursor.close()
			con.close()
		except Exception as e:
			self.logger.error(e)
			return []

		return rows
