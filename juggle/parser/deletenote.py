# 2014-12-17
# build by qianqians
# deletenote

def deletenote(filestr):
    genfilestr = []
    count = 0
    errornote = ""

    for i in xrange(len(filestr)):
        str = filestr[i]

        while(1):
            if count == 1:
                indexafter = str.find("*/")
                if indexafter is not -1:
                    str = str[indexafter+2:]
                    count = 0
                else:
                    break

            index = str.find('//')
            if index is not -1:
                str = str[0:index]
            else:
                indexbegin = str.find("/*")
                if indexbegin is not -1:
                    errornote = str
                    indexafter = str.find("*/")
                    if indexafter is not -1:
                        str = str[0:indexbegin] + str[indexafter+2:]
                    else:
                        count = 1
                        break

            if str is not "":
                genfilestr.append(str)

            break

    if count is 1:
        raise Exception("c/c++ coding error unpaired /* ", errornote)

    return genfilestr
