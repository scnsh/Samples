using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Text;

namespace KakeboConverter
{
    /// <summary>
    /// 家計簿アプリ「かけ～ぼ」から
    /// 別の家計簿アプリ「Zaim」向けにデータを変換します
    /// 入力：かけ～ぼで出力したcsvファイル(cashbook.csv)
    /// 出力：ZaimでインポートできるZaim形式のcsv
    /// 注意：
    ///   - かけ～ぼの限られたデータのみを移行します
    ///     - 移行するデータの条件
    ///       -   デフォルトの帳簿(かけ～ぼ)に記載されている
    ///       -   支払い方法が現金(カード支払いでない)
    ///       -   支出(収入は含まない)
    ///   - かけ～ぼの費目をZaimのカテゴリとして登録
    ///     - ジャンルは「その他」になります
    ///     - 費目は最初からあるものしか対応していません
    /// </summary>
    class Program
    {
        public struct KakeiboItem
        {
            //1列目：日付
            //2列目：費目
            //3列目：収支区分
            //4列目：金額
            //5列目：メモ
            //6列目：帳簿コード
            //7列目：支払いコード
            //8列目：請求日と支払い回数
            //9列目：帳簿間送金
            public DateTime hizuke;
            public string himoku;
            public string syushi;
            public int kingaku;
            public string memo;
            public int tyobo;
            public int shiharai;
            public string seikyu;
            public string sokin;

            public KakeiboItem(string itemLine)
            {
                itemLine = itemLine.Replace("\"", "");
                string[] list = itemLine.Split( new char[]{','});
                if (list.Length != 9)
                {
                    System.Console.WriteLine("size is wrong.");
                }
                if(!DateTime.TryParse(list[0], out hizuke))
                {
                    hizuke = new DateTime(
                        Int32.Parse(list[0].Substring(0, 4)),
                        Int32.Parse(list[0].Substring(4, 2)),
                        Int32.Parse(list[0].Substring(6, 2)));
                }
                himoku = list[1];
                syushi = list[2];
                kingaku = Int32.Parse(list[3]);
                memo = list[4];
                tyobo = Int32.Parse(list[5]);
                shiharai = Int32.Parse(list[6]);
                seikyu = list[7];
                sokin = list[8];
            }

            //一行にまとめる
            public string ToLine()
            {
                string ret = hizuke.ToShortDateString();
                ret += "," + himoku;
                ret += "," + syushi;
                ret += "," + kingaku.ToString();
                ret += "," + memo;
                ret += "," + tyobo.ToString();
                ret += "," + shiharai.ToString();
                ret += "," + seikyu;
                ret += "," + sokin;
                return ret;
            }

            /// <summary>
            /// かけ～ぼで出力したデータをZaimの形式に変換する
            /// </summary>
            /// <returns></returns>
            public string ToZaimLineFromMain()
            {
                string ret = 
                    hizuke.Year.ToString() + "-" + 
                    hizuke.Month.ToString() + "-" +   
                    hizuke.Day.ToString();                // 日付

                if (this.tyobo == 0)                      // 方法
                    ret += ",payment";
                else
                    System.Console.WriteLine("no cash.");
                ret += "," + ConvertCategory(this.himoku);      // カテゴリ
                ret += ",";                                // ジャンル(当該するものがないのでからとする)
                ret += ",お財布";                          // 支払元(手元の移動に固定)
                ret += ",-";                               // 入金先(からのまま)
                ret += "," + this.memo;                         // 商品名(コメントを使う)
                ret += ",";                                // メモ(からにする)
                ret += ",-";                               // 場所(からにする)
                ret += ",（プレミアム会員で表示）";        // 通貨
                ret += "," + (this.syushi == "収入" ? kingaku.ToString() : "0");
                ret += "," + (this.syushi == "支出" ? kingaku.ToString() : "0");
                ret += ",（プレミアム会員で表示）";        // 振替
                ret += ",（プレミアム会員で表示）";        // 残高調整
                ret += ",（プレミアム会員で表示）";        // 通貨返還前の金額
                ret += ",（プレミアム会員で表示）";        // 集計の設定
                return ret;
            }

            /// <summary>
            /// かけ～ぼの費目をZaimのカテゴリにコンバートする
            /// </summary>
            /// <param name="himoku"></param>
            /// <returns></returns>
            public static string ConvertCategory(string himoku)
            {
                switch(himoku)
                {
                    case "食費":
                    case "交通費":
                        return "交通";
                    case "光熱費":
                        return "水道・高熱";
                    case "通信費":
                        return "通信";
                    case "趣味・娯楽費":
                    case "嗜好品":
                    case "こづかい":
                        return "エンタメ";
                    case "生活費":
                        return "日用雑貨";
                    case "教育費":
                        return "教育・教養";
                    case "交際費":
                        return "交際費";
                    case "医療費":
                        return "医療・保険";
                    case "税金":
                        return "税金";
                    case "服飾":
                        return "美容・衣服";
                    case "その他":
                        return "その他";
                }
                Console.WriteLine("費目がただしく変換されませんでした.");
                return "その他";
            }

            /// <summary>
            /// Zaimで出力されるCSVのフォーマットを出力
            /// </summary>
            /// <returns></returns>
            public static string ZaimFirstLine()
            {
                return "日付,方法,カテゴリ,ジャンル,支払元,入金先,商品,メモ,場所,通貨,収入,支出,振替,残高調整,通貨変換前の金額,集計の設定";
            }
        }

        static void Main(string[] args)
        {
            if(args.Length != 1)
                System.Console.WriteLine("no args.");
            else
            {
                LoadFromConvertedKakeibo(args[0]);
            }

        }

        /// <summary>
        /// 読み込んで変換して出力する
        /// </summary>
        /// <param name="readFileName"></param>
        static void LoadFromConvertedKakeibo(string readFileName)
        {
            Encoding readEncoding = Encoding.GetEncoding("Shift_JIS");;
            Encoding writeEncoding = Encoding.GetEncoding("Shift_JIS");

            ///読み込み
            StreamReader sr = new StreamReader(readFileName, readEncoding);
            string fileName = readFileName.Substring(0, readFileName.Length - 4);

            //1行目
            string firstLine = sr.ReadLine();
            string[] firstLineList = firstLine.Split( new char[]{','});
            List<KakeiboItem> itemList = new List<KakeiboItem>();

            //2行目以降
            while(sr.Peek() >= 0)
            {
                KakeiboItem item = new KakeiboItem(sr.ReadLine());
                if(item.syushi != "支出")
                {
                    Console.WriteLine(item.himoku + " : 収入は対象外");
                }
                else if (item.tyobo != 0)
                {
                    Console.WriteLine(item.himoku + " : 帳簿は「かけ～ぼ」");
                }
                else if(item.shiharai != 0)
                {
                    Console.WriteLine(item.himoku + " : 支払い方法は現金");
                }
                else
                {//標準のデータ(帳簿0(かけーぼ)に支払い0(現金)で支払いしたデータしか対応していないため
                    itemList.Add(item);
                }
            }

            ///書き出し
            if(itemList.Count > 0)
            {
                //出力するファイルの先頭にZaim_をつける
                StreamWriter sw = new StreamWriter(
                    "Zaim_" + fileName + ".csv"
                    ,false , writeEncoding);
                //1行目を書き込み
                sw.WriteLine(KakeiboItem.ZaimFirstLine());
                for(int index = 0; index < itemList.Count; ++index)
                {
                    //2行目以降の書き込み
                    sw.WriteLine(itemList[index].ToZaimLineFromMain());
                }
                sw.Close();
            }

        }
    }
}
