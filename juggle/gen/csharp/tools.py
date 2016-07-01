# 2016-7-1
# build by qianqians
# tools

def gentypetocsharp(typestr):
    if typestr == 'int':
        return 'Int64'
    elif typestr == 'string':
        return 'String'
    elif typestr == 'array':
        return 'ArrayList'
    elif typestr == 'float':
        return 'Double'
    elif typestr == 'bool':
        return 'Boolean'
    elif typestr == 'table':
        return 'Hashtable'
	
