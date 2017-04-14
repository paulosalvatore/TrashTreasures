using System.Runtime.InteropServices;
using CodeStage.AntiCheat.Common;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEditor;
using UnityEngine;

namespace CodeStage.AntiCheat.EditorCode.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(ObscuredFloat))]
	internal class ObscuredFloatDrawer : ObscuredPropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
		{
			SerializedProperty hiddenValue = prop.FindPropertyRelative("hiddenValue");
			SerializedProperty hiddenValue1 = hiddenValue.FindPropertyRelative("b1");
			SerializedProperty hiddenValue2 = hiddenValue.FindPropertyRelative("b2");
			SerializedProperty hiddenValue3 = hiddenValue.FindPropertyRelative("b3");
			SerializedProperty hiddenValue4 = hiddenValue.FindPropertyRelative("b4");

			SerializedProperty hiddenValueOld = prop.FindPropertyRelative("hiddenValueOld");
			SerializedProperty hiddenValueOld1 = null;
			SerializedProperty hiddenValueOld2 = null;
			SerializedProperty hiddenValueOld3 = null;
			SerializedProperty hiddenValueOld4 = null;

			if (hiddenValueOld != null && hiddenValueOld.isArray && hiddenValueOld.arraySize == 4)
			{
				hiddenValueOld1 = hiddenValueOld.GetArrayElementAtIndex(0);
				hiddenValueOld2 = hiddenValueOld.GetArrayElementAtIndex(1);
				hiddenValueOld3 = hiddenValueOld.GetArrayElementAtIndex(2);
				hiddenValueOld4 = hiddenValueOld.GetArrayElementAtIndex(3);
			}

			SetBoldIfValueOverridePrefab(prop, hiddenValue);

			SerializedProperty cryptoKey = prop.FindPropertyRelative("currentCryptoKey");
			SerializedProperty fakeValue = prop.FindPropertyRelative("fakeValue");
			SerializedProperty inited = prop.FindPropertyRelative("inited");

			int currentCryptoKey = cryptoKey.intValue;

			IntBytesUnion union = new IntBytesUnion();
			float val = 0;

			if (!inited.boolValue)
			{
				if (currentCryptoKey == 0)
				{
					currentCryptoKey = cryptoKey.intValue = ObscuredFloat.cryptoKeyEditor;
				}
				inited.boolValue = true;

				union.i = ObscuredFloat.Encrypt(0, currentCryptoKey);

				hiddenValue1.intValue = union.b4.b1;
				hiddenValue2.intValue = union.b4.b2;
				hiddenValue3.intValue = union.b4.b3;
				hiddenValue4.intValue = union.b4.b4;
			}
			else
			{
				if (hiddenValueOld != null && hiddenValueOld.isArray && hiddenValueOld.arraySize == 4)
				{
					union.b4.b1 = (byte)hiddenValueOld1.intValue;
					union.b4.b2 = (byte)hiddenValueOld2.intValue;
					union.b4.b3 = (byte)hiddenValueOld3.intValue;
					union.b4.b4 = (byte)hiddenValueOld4.intValue;
				}
				else
				{
					union.b4.b1 = (byte)hiddenValue1.intValue;
					union.b4.b2 = (byte)hiddenValue2.intValue;
					union.b4.b3 = (byte)hiddenValue3.intValue;
					union.b4.b4 = (byte)hiddenValue4.intValue;
				}
				

				val = ObscuredFloat.Decrypt(union.i, currentCryptoKey);
			}

			EditorGUI.BeginChangeCheck();
			val = EditorGUI.FloatField(position, label, val);
			if (EditorGUI.EndChangeCheck())
			{
				union.i = ObscuredFloat.Encrypt(val, currentCryptoKey);

				hiddenValue1.intValue = union.b4.b1;
				hiddenValue2.intValue = union.b4.b2;
				hiddenValue3.intValue = union.b4.b3;
				hiddenValue4.intValue = union.b4.b4;

				if (hiddenValueOld != null && hiddenValueOld.isArray && hiddenValueOld.arraySize == 4)
				{
					hiddenValueOld.arraySize = 0;
				}
			}

			fakeValue.floatValue = val;
			ResetBoldFont();
		}

		[StructLayout(LayoutKind.Explicit)]
		private struct IntBytesUnion
		{
			[FieldOffset(0)]
			public int i;

			[FieldOffset(0)]
			public ACTkByte4 b4;
		}
	}
}