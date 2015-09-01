import urllib.parse
import time
import hmac
import hashlib
import base64

import requests


class EventHubHelper(object):

    @staticmethod
    def _sign_string(uri, key_name, private_key):

        expiry = int(time.time() + 1000)  # recommended to add ~15min

        string_to_sign = urllib.parse.quote_plus(uri) + '\n' + str(expiry)
        key = private_key.encode('utf-8')
        string_to_sign = string_to_sign.encode('utf-8')

        signed_hmac_sha256 = hmac.HMAC(key, string_to_sign, hashlib.sha256)
        signature = signed_hmac_sha256.digest()
        signature = base64.b64encode(signature)

        return 'SharedAccessSignature sr=' + urllib.parse.quote_plus(uri) + '&sig=' + urllib.parse.quote(signature) + '&se=' + str(expiry) + '&skn=' + key_name

    @staticmethod
    def _send_request(url, key_name, private_key, payload):
        sas_key = EventHubHelper._sign_string(url, key_name, private_key)
        headers = {'Content-Type': 'application/atom+xml;type=entry;charset=utf-8', 'Authorization': sas_key}

        r = requests.post(url, headers=headers, data=payload)

        print(r.status_code)

    @staticmethod
    def send_publisher_request(service_namespace, hub_name, publisher_name, key_name, private_key, payload):
        url = "https://%s.servicebus.windows.net/%s/publishers/%s/messages" % (service_namespace, hub_name, publisher_name)
        EventHubHelper._send_request(url, key_name, private_key, payload)

    @staticmethod
    def send_request(service_namespace, hub_name, key_name, private_key, payload):
        url = "https://%s.servicebus.windows.net/%s/messages" % (service_namespace, hub_name)
        EventHubHelper._send_request(url, key_name, private_key, payload)
