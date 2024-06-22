class Configuration:
    @staticmethod
    def read_env_file(file_path):
        env_vars = {}
        with open(file_path) as f:
            for line in f:
                line = line.strip()
                if line and not line.startswith('#'):
                    key, value = line.split('=', 1)
                    value = value.strip('"')
                    env_vars[key] = value
        return env_vars