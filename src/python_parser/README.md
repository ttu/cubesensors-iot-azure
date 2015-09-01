### VirtualEnv (Windows)

When having multiple Python versions installed, define which one to use when creating VirtualEnv

```sh
C:\Python34\Scripts\virtualenv.exe env
env\Scripts\activate
pip install -r requirements.txt
python XXXX.py
env\Scripts\deactivate
```

### Tests

Exeute tests with nose. Use -s if you want to see prints in output.

```sh
nosetests -s
```

Execute single tests

```sh
nosetests -s tests/cube_parser_tests.py:CubeParserTests.test_current_real_data
```
