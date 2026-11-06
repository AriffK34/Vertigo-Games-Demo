using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using VertigoDemo.Data;

namespace VertigoDemo.Tests
{
    public class WheelConfigTests
    {
        #region Fields

        private const int TEST_SLICE_COUNT = 8;

        private ItemData _reward;
        private ItemData _bomb;
        private WheelConfig _wheel;

        #endregion

        #region Setup and Cleanup

        [SetUp]
        public void SetUp()
        {
            _reward = ScriptableObject.CreateInstance<ItemData>();
            _bomb = ScriptableObject.CreateInstance<ItemData>();
            _wheel = ScriptableObject.CreateInstance<WheelConfig>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_wheel);
            Object.DestroyImmediate(_reward);
            Object.DestroyImmediate(_bomb);
        }

        #endregion

        #region Tests

        [TestCase(true, 1)]
        [TestCase(false, 0)]
        public void ValidateConfiguration_WithCorrectBombCount_AcceptsWheel(bool hasBomb, int bombCount)
        {
            ConfigureWheel(bombCount);
            Assert.DoesNotThrow(() => _wheel.ValidateConfiguration(hasBomb));
        }

        [TestCase(true, 0)]
        [TestCase(true, 2)]
        [TestCase(false, 1)]
        public void ValidateConfiguration_WithIncorrectBombCount_RejectsWheel(bool hasBomb, int bombCount)
        {
            ConfigureWheel(bombCount);
            Assert.Throws<System.InvalidOperationException>(
                () => _wheel.ValidateConfiguration(hasBomb));
        }

        #endregion

        #region Helpers

        private void ConfigureWheel(int bombCount)
        {
            SerializedObject serializedWheel = new SerializedObject(_wheel);
            SerializedProperty slices = serializedWheel.FindProperty("_wheelSlices");
            slices.arraySize = TEST_SLICE_COUNT;
            
            for (int i = 0; i < TEST_SLICE_COUNT; i++)
            {
                SerializedProperty slice = slices.GetArrayElementAtIndex(i);
                bool isBomb = i < bombCount;
                slice.FindPropertyRelative("itemData").objectReferenceValue = isBomb ? _bomb : _reward;
                slice.FindPropertyRelative("itemAmount").intValue = isBomb ? 0 : 1;
                slice.FindPropertyRelative("_isBomb").boolValue = isBomb;
            }

            serializedWheel.ApplyModifiedPropertiesWithoutUndo();
        }

        #endregion
    }
        
}

