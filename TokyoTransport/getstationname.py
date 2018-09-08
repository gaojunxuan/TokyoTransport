import urllib.request
import json
import sqlite3
db=sqlite3.connect("data.sqlite",check_same_thread=False)
cursor=db.cursor()
line_db=sqlite3.connect("line.sqlite",check_same_thread=False)
line_cursor=line_db.cursor()
def getStationName(company):
    contents = urllib.request.urlopen("https://api-tokyochallenge.odpt.org/api/v4/odpt:Railway?odpt:operator=odpt.Operator:{}&acl:consumerKey=ae53c851f319cd3b5d181eb91d6163a5cd93770d51d4e54a51324df11d5fa92e".format(company)).read()
    jobj=json.loads(contents)
    sql_create_projects_table = """ CREATE TABLE IF NOT EXISTS stations (
                                            name text,
                                            company text,
                                            line text,
                                            ja text,
                                            en text
                                        ); """
    cursor.execute(sql_create_projects_table)
    for i in jobj:
        staOrder=i["odpt:stationOrder"]
        for j in staOrder:
            name=j["odpt:station"].split(".")[-1]
            company=i["owl:sameAs"].split(".")[-2].replace("Railway:","")
            line=i["owl:sameAs"].split(".")[-1]
            sql = ''' INSERT INTO stations(name,company,line,ja,en)
                VALUES(?,?,?,?,?) '''
            cursor.execute(sql,(name,company,line,j["odpt:stationTitle"]["ja"],j["odpt:stationTitle"]["en"]))
            print(name)

    db.commit()

def getLineName(company):
    contents = urllib.request.urlopen("https://api-tokyochallenge.odpt.org/api/v4/odpt:Railway?odpt:operator=odpt.Operator:{}&acl:consumerKey=ae53c851f319cd3b5d181eb91d6163a5cd93770d51d4e54a51324df11d5fa92e".format(company)).read()
    jobj=json.loads(contents)
    sql_create_projects_table = """ CREATE TABLE IF NOT EXISTS lines (
                                            name text,
                                            ja text,
                                            en text,
                                            code text,
                                            company text
                                        ); """
    line_cursor.execute(sql_create_projects_table)
    for i in jobj:
        sql = ''' INSERT INTO lines(name,ja,en,code,company)
                VALUES(?,?,?,?,?) '''
        name=i["owl:sameAs"].split(".")[-1]
        ja=i["odpt:railwayTitle"]["ja"]
        en=i["odpt:railwayTitle"]["en"]
        if "odpt:lineCode" in i:
            code=i["odpt:lineCode"]
        else:
            code=""
        line_cursor.execute(sql,(name,ja,en,code,company))

    line_db.commit()   
    
company=["JR-East","Keikyu","Keio","Keisei","Odakyu","Seibu","Tobu","Toei","TokyoMetro","TWR","Yurikamome"]
for c in company:
    getStationName(c)