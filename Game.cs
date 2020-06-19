using Sequence = System.Collections.IEnumerator;

/// <summary>
/// ゲームクラス。
/// 学生が編集すべきソースコードです。
/// </summary>
public sealed class Game : GameBase
{
    // 変数の宣言
    //int sec = 0;
    //変数宣言。x座標とy座標。スピードの変数。
    bool fin = false;

    int ball_x;
    int ball_y;
    int ball_speed_x;
    int ball_speed_y;

    //ラケットのx座標、y座標、幅、高さ
    int player_x;
    int player_y;
    int player_w;
    int player_h;


    const int BLOCK_NUM = 50;
    int[] block_x = new int[BLOCK_NUM];
    int[] block_y = new int[BLOCK_NUM];
    bool[] block_alive_flag = new bool[BLOCK_NUM];
    int block_w = 64;
    int block_h = 20;
    int time;


    /// <summary>
    /// 初期化処理
    /// </summary>
    public override void InitGame()
    {
        // キャンバスの大きさを設定
        gc.SetResolution(640, 480);
        ball_x = 0;
        ball_y = 100;
        ball_speed_x = 5;
        ball_speed_y = 5;

        player_x = 270;
        player_y = 460;
        player_w = 100;
        player_h = 20;

        time = 0;

  
        //50回ループさせる
        for (int i = 0; i < BLOCK_NUM; i++)
        {
            //％をうまく使ってブロックの位置決めをする
            block_x[i] = (i % 10) * block_w;
            block_y[i] = (i / 10) * block_h;
            //ブロックがあると言うこと
            block_alive_flag[i] = true;
        }

       
    }

    /// <summary>
    /// 動きなどの更新処理
    /// </summary>
    public override void UpdateGame()
    {
        
        //ボールの座標にball_speed変数を足して座標を変化させている
        ball_x = ball_x + ball_speed_x;
        ball_y = ball_y + ball_speed_y;

   
            //ボールが端に当たった時の操作
            if (ball_x < 0 )
            {
            //これ以上値が小さくなってしまわないようにしている。
                ball_x = 0;
                ball_speed_x = -ball_speed_x;
            }

            if (ball_y < 0 )
            {
                ball_y = 0;
                ball_speed_y = -ball_speed_y;
            }

            if (ball_x > 616 )
            {
                //この時点でball_xは６１６を超えていると言うことなので、ball_speed_xをーにしてだんだん減っていくようにする。
                ball_x = 616;
                ball_speed_x = -ball_speed_x;

            }

        
            //課題２
            //マウスがタッチされている時
            if (gc.GetPointerFrameCount(0) > 0)
        　　{
            player_x = gc.GetPointerX(0) - player_w / 2;
            player_y = gc.GetPointerY(0) - player_h / 2;
        　　}

            //もしボールがplayerに当たったら
            if (gc.CheckHitRect(ball_x,ball_y,24,24,player_x,player_y,player_w,player_h))
            {
              if (ball_speed_y > 0)
              {
                ball_speed_y = -ball_speed_y;
              }

            //課題５ playerがだんだん小さくなって、最小wが３０になるように設定。
            player_w = player_w - 7;

            if (player_w < 30)
            {
                player_w = 30;
            }

        }

            //countBlockが存在していればtime+1する
            if(countBlock() !=0)
            {
              time = time + 1;
            }

        　　for (int i = 0; i < BLOCK_NUM; i++)
        　　{
           　　 if (gc.CheckHitRect(ball_x, ball_y, 15, 15, block_x[i], block_y[i], block_w, block_h))
           　　 {

                //課題５　ブロックに一つずつ当たったら跳ね返る
                if(block_alive_flag[i] == true)
                {
                    ball_speed_x = -ball_speed_x;
                    ball_speed_y = -ball_speed_y;
                }
                //あたったらその場所のブロックを消す。
                block_alive_flag[i] = false;

            　　}
           }
    }

    /// <summary>
    /// 描画の処理
    /// </summary>
    public override void DrawGame()
    {
        // 画面を白で塗りつぶします
        gc.ClearScreen();
        //この先のプログラム内において、色を使う時の配色を設定
        gc.SetColor(80, 50, 150);

        //0番目の画像をx＝０、y＝０で配置
        //gc.DrawImage(0, 0, 0);
        gc.DrawImage(2, ball_x, ball_y);
        //player変数の値に剃って塗りつぶす。
        gc.FillRect(player_x, player_y, player_w, player_h);

        //もしblock_alive_flag[i]があればその場所を塗りつぶしてブロックを精製
        for (int i = 0; i < BLOCK_NUM; i++)
        {
            if(block_alive_flag[i] == true)
            {
                gc.FillRect(block_x[i], block_y[i], block_w, block_h);
            }
        }


        if(fin == false || countBlock() ==0)
        {
            gc.DrawString("time" + time, 60, 150);
        }

        //終了時動作
        if (countBlock() == 0)
        {
            gc.DrawString("clear!" + "ボールを外に出してから長押しでrestart!", 50, 174);
           
            fin = true;
            
        }

        //もしブロックを落としてしまって、かつブロックがある状態ならゲームオーバー処理をする
        if(ball_y > 500 && countBlock() != 0)
        {
            gc.DrawString("gameover!!" + "長押しでrestart!", 60, 174);
            fin = true;
        }

        //もしゲームが何らかの形で終了していて、かつブロックがフレームアウトした状態なら、リセット処理をする
        if(fin == true && ball_y >500)
        {
            if(gc.GetPointerFrameCount(0) > 120 )
            {
                InitGame();
                fin = false;
            }
        }



    }


    //ブロックを数える
    int countBlock()
    {
        int num = 0;
        for (int i = 0; i < BLOCK_NUM; i++)
        {
            if (block_alive_flag[i])
            {
                num++;
            }
        }
        return num;
    }

   
}


///あと追加すること
//・ゲームクリアした後のrestart \
//・ブロック一つ一つを跳ね返らせる。
//・ボールの速度を上げてみる。\
//・player幅を狭くする。＼
