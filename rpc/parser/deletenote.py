#coding:utf-8
# 2014-12-17
# build by qianqians
# deletenote

def deletenote(filestr):
    genfilestr = []
    count = 0
    errornote = ""

    for i in range(len(filestr)):
        _str = filestr[i]

        while(1):
            if count == 1:
                indexafter = _str.find("*/")
                if indexafter is not -1:
                    _str = _str[indexafter+2:]
                    count = 0
                else:
                    break

            index = _str.find('//')
            if index is not -1:
                _str = _str[0:index]
            else:
                indexbegin = _str.find("/*")
                if indexbegin is not -1:
                    errornote = _str
                    indexafter = _str.find("*/")
                    if indexafter is not -1:
                        _str = _str[0:indexbegin] + _str[indexafter+2:]
                    else:
                        count = 1
                        break

            if _str is not "":
                genfilestr.append(_str)

            break

    if count is 1:
        raise Exception("c/c++ coding error unpaired /* ", errornote)

    return genfilestr
