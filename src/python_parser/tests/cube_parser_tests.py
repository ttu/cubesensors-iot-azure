import unittest
import json

from python_parser.cube_parser import CubeParser


class CubeParserTests(unittest.TestCase):

    current_sample = """
    {"results": [["2015-08-29T21:50:00Z", 2265, 1012, 55, 450, 0, null, 45, 58, false, false, 72312, -83]], "field_list": ["time", "temp", "pressure", "humidity", "voc", "light", "noise", "noisedba", "battery", "shake", "cable", "voc_resistance", "rssi"], "ok": true}
    """

    def test_current_sample_data(self):
        cube_id = "test-123-X"
        full_js = json.loads(self.current_sample)

        parser = CubeParser()

        result = parser.parse_current_result(cube_id, full_js, True)
        self.assertEqual(result["id"], cube_id)

    @unittest.skip("Use real API only on demand")
    def test_current_real_data(self):
        datas = CubeParser().get_data()
        print(datas)