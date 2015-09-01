# keys file is not included in repository
import keys

ROOT = "http://api.cubesensors.com"
AUTH = "%s/auth" % ROOT
RES = "%s/v1" % ROOT

# CubeSensors config
consumer_key = keys.cube_consumer_key
consumer_secret = keys.cube_consumer_secret

oauth_verifier = keys.cube_oauth_verifier
access_token = keys.cube_access_token
access_token_secret = keys.cube_access_token_secret

# Azure config
servicebus_namespace = keys.azure_servicebus_namespace

eventhub_name = "cubesensor"
eventhub_key_name = "cube_sender"
eventhub_private_key = keys.cubesender_private_key
