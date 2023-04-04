class moduleException(Exception):
	def __init__(self, err:str) -> None:
		super(moduleException, self).__init__(err)
		self.err = err

class Response(object):
	def __init__(self) -> None:
		pass
	
class imodule(object):
	def __init__(self) -> None:
		self.rsp : Response = None