import requests
import bs4
import re
import datetime
# Load twitter tokens from the external twitterTokens.py file.
# Tokens are in the dictionary named "tokens".
from twitterTokens import tokens
import tweepy


d = datetime.datetime.today()
#d -= datetime.timedelta(days=1)
target_date = str(d.month) + "月" + str(d.day) + "日"
url = "http://baseball.yahoo.co.jp/npb/schedule/?&date=" + d.strftime("%Y%m%d")
res = requests.get(url)
yahoo_sports = bs4.BeautifulSoup(res.text.encode(res.encoding), "html.parser")

def isUpdate():
    return True

def getScore():
    carp_name = "広島"
    enemy_name = ""
    carp_score = ""
    enemy_score = ""
    place_name = ""
    win_pitcher_name = ""
    lose_pitcher_name = ""
    save_pitcher_name = ""

    score_list = yahoo_sports.find(id='week_sch').find(class_='yjMS mb5')
    rows = score_list.find_all("tr")
    start_read = False
    is_carp_game = False            
    is_home = False    
    for row in rows:
        #print(row)
        day = row.find("th")
        if day != None:
            if target_date in day.get_text():
                start_read = True
            else:
                start_read = False
        if start_read:
            info = row.find_all("td")
            teams_and_place = row.find_all(class_='today pl7')
            score = row.find(class_='today ct')
            win_pitcher = row.find(class_='w')
            save_pitcher = row.find(class_='s')
            lose_pitcher = row.find(class_='l')
            if teams_and_place != None:
                for i in range(len(teams_and_place)):
                    if teams_and_place[i].get_text() == "広島":
                        is_carp_game = True
                        if i == 0:
                            is_home = True
            if is_carp_game:
                if is_home:
                    enemy_name = teams_and_place[1].get_text()
                    if score.get_text() != "-":
                        carp_score = score.get_text().split(" ")[0]
                        enemy_score = score.get_text().split(" ")[2]
                else:
                    enemy_name = teams_and_place[0].get_text()
                    if score.get_text() != "-":
                        carp_score = score.get_text().split(" ")[2]
                        enemy_score = score.get_text().split(" ")[0]                
                place_name = teams_and_place[2].get_text().split(" ")[1]
                if win_pitcher != None:
                    win_pitcher_name = win_pitcher.get_text()
                if lose_pitcher != None:                   
                    lose_pitcher_name = lose_pitcher.get_text()
                if save_pitcher != None:                   
                    save_pitcher_name = save_pitcher.get_text()

                result = target_date + place_name + " "
                result += carp_name + ":" + carp_score + " - " + enemy_score + ":" + enemy_name + " "
                result += win_pitcher_name + " " + save_pitcher_name + " " + lose_pitcher_name + " "
                return result
    return "試合なし"

def tweetScore():
    # set tokens
    CONSUMER_KEY = tokens['consumer_key']
    CONSUMER_SECRET = tokens['consumer_secret']
    ACCESS_TOKEN = tokens['access_token']
    ACCESS_TOKEN_SECRET = tokens['access_token_secret']
    # auth process
    auth = tweepy.OAuthHandler(CONSUMER_KEY, CONSUMER_SECRET)
    auth.set_access_token(ACCESS_TOKEN, ACCESS_TOKEN_SECRET)
    api = tweepy.API(auth)
    # send tweet
    api.update_status(getScore() + ' #carp_score ')

def lambda_handler(event, context):
    if isUpdate():
        tweetScore()

if __name__ == '__main__':
    lambda_handler(None, None)