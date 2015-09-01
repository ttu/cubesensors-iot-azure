import sys

from rauth import OAuth1Service
from rauth import OAuth1Session

import config


class CubeSession(object):

    @staticmethod
    def get_session():

        if config.access_token == "":
            print("Execute cuber_session.py to get access token and update config.py")
            sys.exit(0)

        session = OAuth1Session(
            config.consumer_key,
            config.consumer_secret,
            access_token=config.access_token,
            access_token_secret=config.access_token_secret)

        return session

    # This needs to be done before API can be used
    @staticmethod
    def get_access_token():
        cbsr = OAuth1Service(
            consumer_key=config.consumer_key,
            consumer_secret=config.consumer_secret,
            access_token_url='%s/access_token' % config.AUTH,
            authorize_url='%s/authorize' % config.AUTH,
            request_token_url='%s/request_token' % config.AUTH,
            base_url='%s/' % config.RES)

        request_token, request_token_secret = cbsr.get_request_token(params={"oauth_callback": "oob"})

        authorize_url = cbsr.get_authorize_url(request_token)
        print("Go to %s" % authorize_url)
        oauth_verifier = input('Enter oauth_verifier: ')

        access_token, access_token_secret = cbsr.get_access_token(
                request_token,
                request_token_secret,
                method="POST",
                params={"oauth_verifier": oauth_verifier})

        print("Add these to keys.py")
        print("oauth_verifier: %s\nAccess Token: %s\nAccess Token Secret: %s" % (oauth_verifier, access_token, access_token_secret))


if __name__ == '__main__':
    CubeSession.get_access_token()
