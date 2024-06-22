import urequests as requests

class SaveData:
    def __init__(self, api_token):
        self.api_token = api_token
        pass
    
    def get_token(self, username, password):
        url = f'https://plantobserverapi.azurewebsites.net/PlantData/token?user={username}&password={password}'
        headers = {
            "Accept": 'text/plain'
        }
        params = {
            'user' : username,
            'password' : password
        }
        try:
            response = requests.post(url, headers=headers, json=params)
            if response.status_code == 200:
                return response.text
            else:
                print(response.status_code)
        except Exception as e:
            print(e)
        finally:
            if response:
                response.close()

    def build_json(self, variable, value):
        try:
            data = {variable: {"value": value}}
            return data
        except:
            return None

    def sendData(self, device, variable, value):
        try:
            url = "https://industrial.api.ubidots.com/"
            url = url + "api/v1.6/devices/" + device
            headers = {"X-Auth-Token": self.api_token, "Content-Type": "application/json"}
            data = self.build_json(variable, value)

            if data is not None:
                print("senddata.py ok",data)
                req = requests.post(url=url, headers=headers, json=data)
                return req.json()
            else:
                pass
        except:
            pass
        
    def send_to_api(self, token, temperature):
        url = f'https://plantobserverapi.azurewebsites.net/PlantData/post'
        headers = {
            'Content-Type': 'application/json',
            'Authorization': f'Bearer {token}'
        }
        data = {
            "temperature" : temperature
        }
        try:
            req = requests.post(url, headers=headers, json=data)
            if req.status_code == 200:
                print("save_data.py Post successful")
            else:
                print(req.status_code)
        except Exception as e:
            print(e)
        finally:
            if req:
                req.close()