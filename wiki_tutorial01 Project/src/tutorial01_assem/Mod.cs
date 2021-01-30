using System;
using System.Collections.Generic;
using Modding;
using Modding.Blocks;
using UnityEngine;

namespace Tutorial01Space
{
	// mod�̋��_�ƂȂ�N���X
	public class Mod : ModEntryPoint
	{
		// �ϐ�
		GameObject mod;

		// ���\�b�h
		// �X�e�[�W�i�o�����G�N�X�p���X�Ȃǁj�ɏ��߂ē���������mod�����[�h����A������OnLoad()���Ăяo�����B
		public override void OnLoad()
		{
			// �R���\�[���ɁuHello World!�v�ƕ\��������B
			ModConsole.Log("Hello World!");

			// mod�Q�[���I�u�W�F�N�g�����������AGUI���Ŏg���N���X�̃C���X�^���X���q�Ɏw�肷��B
			mod = new GameObject("WikiTutorialController");
			SingleInstance<Gui>.Instance.transform.parent = mod.transform;
			SingleInstance<BlockSelector>.Instance.transform.parent = mod.transform;

			// �V�[�����܂����ł�mod��������Ȃ��悤�ɂ���B
			UnityEngine.Object.DontDestroyOnLoad(mod);
		}

		// ���̂悤�ȃv�����g�n���\�b�h��p�ӂ��Ă����Ƒ�mod�̃��b�Z�[�W�ƍ������ɂ����B
		public static void Log(string msg)
        {
			// ModConsole.Log(...) �Ə����Ă�����
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

	// GUI�ŗp����N���X
	// SingleInstance<>���p������͎̂������Ǝv���ĖႦ��OK
	public class Gui : SingleInstance<Gui>
    {
		// �ϐ�
		private Rect windowRect = new Rect(0, 80, 200, 100);
		private int windowId;
		public bool StartingBlockToggle = false;

		// �v���p�e�B
		// SingleInstance<>�̌p���ŕK�v�����A�ǂ�Ȗ��O�ł��悢
		public override string Name
        {
			get
            {
				return "wiki tutorial gui";
            }
        }

		// ���\�b�h
		public void Awake()
        {
			// ����mod��GUI�Ƌ������Ȃ�window ID�𐶐�����i�������Ǝv����OK�j
			windowId = ModUtility.GetWindowId();
        }

		// GUI�̒l���ω�����t���[���ł̂݌Ăяo�����
		// ���̂��߁AUpdate�����v�Z�ʂ����Ȃ��Ȃ�
		public void OnGUI()
        {
			if (!StatMaster.isClient && !StatMaster.isMainMenu)
            {
				windowRect = GUILayout.Window(windowId, windowRect, delegate(int windowId)
				{
					// GUI�̒��g���\������

					// ������\��������
					GUILayout.Label("Mod���S�ɗ�������");

					// �X�^�[�g�u���b�N�̃g�O����������Ă���΁A������\������
					if (StartingBlockToggle)
					{
						GUILayout.Label("�X�^�u���Ƙa������");
					}

					GUI.DragWindow();
				}
				, "wiki tutorial");
            }
        }
    }

	// �u���b�N�ɃX�N���v�g��ǉ�����N���X
	public class BlockSelector : SingleInstance<BlockSelector>
    {
		// �ϐ�
		// �u���b�N��ID�ƁA�ǉ��������X�N���v�g��R�Â�������
		public Dictionary<int, Type> BlockDict = new Dictionary<int, Type>
		{
			// �X�^�[�g�u���b�N
			{0, typeof(StartingBlockScript) },
		};

		// �v���p�e�B
		// Gui�Ɠ��l�̎���
		public override string Name
        {
            get
            {
				return "wiki tutorial BlockSelector";
            }
        }

		// ���\�b�h
		public void Awake()
        {
			// �u���b�N��ݒu�����ꍇ�ɌĂяo�����A�N�V�����ɁAAddScript�Ƃ������\�b�h��ǉ�����
			Events.OnBlockInit += new Action<Block>(AddScript);
        }

		// �u���b�N�ݒu���ɁA���̃u���b�N�ɏ���̃X�N���v�g��\��t����֐�
		// Block�́A�ݒu�����u���b�N��\��
		public void AddScript(Block block)
        {
			// ���������u���b�N��BlockBehaviour�R���|�[�l���g���擾����
			BlockBehaviour internalObject = block.BuildingBlock.InternalObject;

			// ���̃u���b�N���X�N���v�g��\��t����ׂ��u���b�N�ł���Ȃ�A�\��t����
			if (BlockDict.ContainsKey(internalObject.BlockID))
            {
				Type type = BlockDict[internalObject.BlockID];
                try
                {
					// �܂�����̃X�N���v�g���\��t�����Ă��Ȃ��ꍇ�ɂ̂݁A�\��t����
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

	// �X�^�[�g�u���b�N�ɓ\��t����X�N���v�g
	public class StartingBlockScript : MonoBehaviour
    {
		// �ϐ�
		public BlockBehaviour BB;
		public MToggle toggle;

        // ���\�b�h
        private void Awake()
        {
			// BlockBehaviour���擾
			BB = GetComponent<BlockBehaviour>();

			// �u���b�N�̐ݒ��ʂɃg�O����ǉ�
			toggle = BB.AddToggle("toggle", "wiki tutorial toggle", false);
        }

		private void Update()
        {
			// GUI�ŕ��͂�\�����邩���A�g�O���̒l�ɉ����ĕς���
			SingleInstance<Gui>.Instance.StartingBlockToggle = toggle.IsActive;
        }
    }
}
