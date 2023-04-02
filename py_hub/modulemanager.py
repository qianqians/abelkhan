import module
from collections.abc import Callable

class modulemanager(object):
	def __init__(self) -> None:
		self.motheds : dict[str, Callable[[list]]] = {}

	def add_mothed(self, cb_name:str, method:Callable[[list]]):
		self.motheds[cb_name] = method

	def process_module_mothed(self, func_name:str, argvs:list):
		_method = self.motheds.get(func_name)
		if _method:
			_method(argvs)
		else:
			print(f"do not have a mothed name:{func_name}")
			raise module.moduleException(f"do not have a mothed name:{func_name}")