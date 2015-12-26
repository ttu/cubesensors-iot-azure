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

    def parse_to_db_format(self, payload):
        parsed_datas = []

        for data in payload:
            parsed_datas.append((data['id'], data['time'], data['temp'], data['pressure'], data['humidity'], data['voc'], data['light'], data['noisedba'], data['battery'], data['cable'], data['voc_resistance'], data['rssi']))

        return parsed_datas


    def send_request(self, data_array):

        print(data_array)
        
        stmt = '''
        INSERT INTO cubesensors_data VALUES (%s,%s,%s,%s,%s,%s,%s,%s,%s,%s,%s,%s)
        '''

        try:
            con = pymssql.connect(server=self.server, user=self.user_name, password=self.password, database = self.db_name)
            cursor = con.cursor()
            cursor.executemany(stmt, data_array)
            con.commit()
            con.close()
        except Exception as e:
            self.logger.error(e)
            return False

        return True