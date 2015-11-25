import random
import string
import logging

import pypyodbc

import config


class DataStore(object):

	def __init__(self,  db_server, db_name, user_name, password):
		self.logger = logging.getLogger(config.logger_name)
		self.con_string = "Driver={SQL Server}; Server=tcp:" + db_server + ".database.windows.net,1433; Database=" + db_name + "; Uid=" + user_name + ";Pwd=" + password + ";"

	def parse_to_db_format(self, payload):
		parsed_datas = []
		
		for data in payload:
			parsed_datas.append([data['id'], data['time'], data['temp'], data['pressure'], data['humidity'], data['voc'], data['light'], data['noisedba'], data['battery'], data['cable'],  data['voc_resistance'], data['rssi']])
	
		return parsed_datas
	
	
	def send_request(self, data_array):
	
		stmt = '''
		INSERT INTO cubesensors_data VALUES (?,?,?,?,?,?,?,?,?,?,?,?)
		'''

		try:
			con = pypyodbc.connect(self.con_string)
			cursor = con.cursor()
			cursor.executemany(stmt, data_array)
			cursor.commit()
			cursor.close()
			con.close()
		except Exception as e:
			self.logger.error(e)
			return False

		return True