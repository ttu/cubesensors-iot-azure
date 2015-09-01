import logging
import config
from cube_session import CubeSession


class CubeParser(object):

    def __init__(self):
        self.logger = logging.getLogger('cubesensors')

    def parse_current_result(self, id, full_json, is_current):
        results = full_json["results"]

        if is_current:
            if len(results) == 0:
                self.logger.error("No results found.")
                return []
            elif len(results) > 1:
                self.logger.warning("Too many results found. Using only first.")

        zipped = dict(zip(full_json["field_list"], results[0]))
        zipped['id'] = id

        return zipped

    def get_devices(self):
        session = CubeSession.get_session()
        response = session.get("%s/devices/" % config.RES)
        result = response.json()
        # print(response.headers)
        # print(result)
        return result

    def get_data(self):
        result = self.get_devices()
        parsed_datas = []
        if result["ok"]:
            for device in result["devices"]:
                cube_data_response = session.get('%s/devices/%s/current' % (config.RES, device["uid"]))
                parsed = self.parse_current_result(device["uid"], cube_data_response.json(), True)
                parsed_datas.append(parsed)
        else:
            self.logger.error("Failed to get devices %s", str(result))

        return parsed_datas


if __name__ == '__main__':
    CubeParser().get_data()
