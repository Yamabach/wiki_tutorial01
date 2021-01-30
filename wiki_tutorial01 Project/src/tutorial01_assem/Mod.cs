using System;
using System.Collections.Generic;
using Modding;
using Modding.Blocks;
using UnityEngine;

namespace Tutorial01Space
{
	// modの拠点となるクラス
	public class Mod : ModEntryPoint
	{
		// 変数
		GameObject mod;

		// メソッド
		// ステージ（バレンエクスパンスなど）に初めて入った時にmodがロードされ、同時にOnLoad()が呼び出される。
		public override void OnLoad()
		{
			// コンソールに「Hello World!」と表示させる。
			ModConsole.Log("Hello World!");

			// modゲームオブジェクトを初期化し、GUI等で使うクラスのインスタンスを子に指定する。
			mod = new GameObject("WikiTutorialController");
			SingleInstance<Gui>.Instance.transform.parent = mod.transform;
			SingleInstance<BlockSelector>.Instance.transform.parent = mod.transform;

			// シーンをまたいでもmodが消されないようにする。
			UnityEngine.Object.DontDestroyOnLoad(mod);
		}

		// このようなプリント系メソッドを用意しておくと他modのメッセージと混同しにくい。
		public static void Log(string msg)
        {
			// ModConsole.Log(...) と書いても同じ
			Debug.Log("wiki tutorial : " + msg);
        }
		public static void Warning(string msg)
        {
			Debug.LogWarning("wiki tutorial : " + msg);
        }
		public static void Error(string msg)
        {
			Debug.LogError("wiki tutorial : " + msg);
        }
	}

	// GUIで用いるクラス
	// SingleInstance<>を継承するのは呪文だと思って貰えばOK
	public class Gui : SingleInstance<Gui>
    {
		// 変数
		private Rect windowRect = new Rect(0, 80, 200, 100);
		private int windowId;
		public bool StartingBlockToggle = false;

		// プロパティ
		// SingleInstance<>の継承で必要だが、どんな名前でもよい
		public override string Name
        {
			get
            {
				return "wiki tutorial gui";
            }
        }

		// メソッド
		public void Awake()
        {
			// 他のmodのGUIと競合しないwindow IDを生成する（呪文だと思ってOK）
			windowId = ModUtility.GetWindowId();
        }

		// GUIの値が変化するフレームでのみ呼び出される
		// そのため、Updateよりも計算量が少なくなる
		public void OnGUI()
        {
			if (!StatMaster.isClient && !StatMaster.isMainMenu)
            {
				windowRect = GUILayout.Window(windowId, windowRect, delegate(int windowId)
				{
					// GUIの中身を構成する

					// 文字を表示させる
					GUILayout.Label("Mod完全に理解した");

					// スタートブロックのトグルが押されていれば、これも表示する
					if (StartingBlockToggle)
					{
						GUILayout.Label("スタブロと和解せよ");
					}

					GUI.DragWindow();
				}
				, "wiki tutorial");
            }
        }
    }

	// ブロックにスクリプトを追加するクラス
	public class BlockSelector : SingleInstance<BlockSelector>
    {
		// 変数
		// ブロックのIDと、追加したいスクリプトを紐づけた辞書
		public Dictionary<int, Type> BlockDict = new Dictionary<int, Type>
		{
			// スタートブロック
			{0, typeof(StartingBlockScript) },
		};

		// プロパティ
		// Guiと同様の呪文
		public override string Name
        {
            get
            {
				return "wiki tutorial BlockSelector";
            }
        }

		// メソッド
		public void Awake()
        {
			// ブロックを設置した場合に呼び出されるアクションに、AddScriptというメソッドを追加する
			Events.OnBlockInit += new Action<Block>(AddScript);
        }

		// ブロック設置時に、そのブロックに所定のスクリプトを貼り付ける関数
		// Blockは、設置したブロックを表す
		public void AddScript(Block block)
        {
			// 生成したブロックのBlockBehaviourコンポーネントを取得する
			BlockBehaviour internalObject = block.BuildingBlock.InternalObject;

			// そのブロックがスクリプトを貼り付けるべきブロックであるなら、貼り付ける
			if (BlockDict.ContainsKey(internalObject.BlockID))
            {
				Type type = BlockDict[internalObject.BlockID];
                try
                {
					// まだ所定のスクリプトが貼り付けられていない場合にのみ、貼り付ける
					if (internalObject.GetComponent(type) == null)
                    {
						internalObject.gameObject.AddComponent(type);
						Mod.Log("Added Script");
                    }
                }
                catch
                {
					Mod.Error("AddScript Error!");
                }
				return;
            }
        }
    }

	// スタートブロックに貼り付けるスクリプト
	public class StartingBlockScript : MonoBehaviour
    {
		// 変数
		public BlockBehaviour BB;
		public MToggle toggle;

        // メソッド
        private void Awake()
        {
			// BlockBehaviourを取得
			BB = GetComponent<BlockBehaviour>();

			// ブロックの設定画面にトグルを追加
			toggle = BB.AddToggle("toggle", "wiki tutorial toggle", false);
        }

		private void Update()
        {
			// GUIで文章を表示するかを、トグルの値に応じて変える
			SingleInstance<Gui>.Instance.StartingBlockToggle = toggle.IsActive;
        }
    }
}
