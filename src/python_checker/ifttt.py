import sys
import requests
import config

class IFTTT(object):
	
	url = "https://maker.ifttt.com/trigger/{}/with/key/{}"
	
	@staticmethod
	def sendEvent(api_key, event):
		u = IFTTT.url.format(event, api_key)
		return requests.post(u)
		
		
if __name__ == '__main__':
	sensor_id = sys.argv[1]
	
	if sys.argv[2] == "on":
		print(sensor_id + config.ifttt_event_on)
		IFTTT.sendEvent(config.ifttt_api_key, sensor_id + config.ifttt_event_on)
	elif sys.argv[2] == "off":
		print(sensor_id + config.ifttt_event_off)
		IFTTT.sendEvent(config.ifttt_api_key, sensor_id + config.ifttt_event_off)
	else:
		print("on/off")