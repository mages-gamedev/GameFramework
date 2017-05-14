﻿//----------------------------------------------
// Flip Web Apps: Game Framework
// Copyright © 2016 Flip Web Apps / Mark Hewitt
//
// Please direct any bugs/comments/suggestions to http://www.flipwebapps.com
// 
// The copyright owner grants to the end user a non-exclusive, worldwide, and perpetual license to this Asset
// to integrate only as incorporated and embedded components of electronic games and interactive media and 
// distribute such electronic game and interactive media. End user may modify Assets. End user may otherwise 
// not reproduce, distribute, sublicense, rent, lease or lend the Assets. It is emphasized that the end 
// user shall not be entitled to distribute or transfer in any way (including, without, limitation by way of 
// sublicense) the Assets in any other way than as integrated components of electronic games and interactive media. 

// The above copyright notice and this permission notice must not be removed from any files.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//----------------------------------------------

#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2
#else
using System.Security.Cryptography;
using GameFramework.Localisation.ObjectModel;
using NUnit.Framework;
using UnityEngine;

namespace GameFramework.Localisation
{
    /// <summary>
    /// Test cases for localisation. You can also view these to see how you might use the API.
    /// </summary>
    public class LocalisationTests
    {
        #region Helper Functions

        LocalisationData CreateNewLocalisation()
        {
            return ScriptableObject.CreateInstance<LocalisationData>();
        }

        #endregion Helper Functions

        #region Setup Tests

        [Test]
        public void CreateLocalisation()
        {
            //// Act
            var localisationData = CreateNewLocalisation();

            //// Assert
            Assert.IsNotNull(localisationData, "No LocalisationData was created");
            Assert.IsNull(localisationData.InternalVerifyState(), localisationData.InternalVerifyState());
        }


        #endregion Setup Tests

        #region Language Tests

        [TestCase("English")]
        [TestCase("French")]
        public void AddLanguage(string language)
        {
            // Arrange
            var localisationData = CreateNewLocalisation();

            //// Act
            localisationData.AddLanguage(language);

            //// Assert
            Assert.IsTrue(localisationData.ContainsLanguage(language), "Language not added");
            Assert.IsNull(localisationData.InternalVerifyState(), localisationData.InternalVerifyState());
        }

        [TestCase("English", "French", "Spanish")]
        [TestCase("French", "German", "Spanish")]
        public void AddLanguageMultiple(string language, string language2, string language3)
        {
            // Arrange
            var localisationData = CreateNewLocalisation();

            //// Act
            localisationData.AddLanguage(language);
            localisationData.AddLanguage(language2);
            localisationData.AddLanguage(language3);

            //// Assert
            Assert.IsTrue(localisationData.ContainsLanguage(language), "Language not added");
            Assert.IsTrue(localisationData.ContainsLanguage(language2), "Language2 not added");
            Assert.IsTrue(localisationData.ContainsLanguage(language3), "Language3 not added");
            Assert.IsNull(localisationData.InternalVerifyState(), localisationData.InternalVerifyState());
        }

        [TestCase("English")]
        [TestCase("French")]
        public void AddlanguageNoDuplicates(string language)
        {
            // Arrange
            var localisationData = CreateNewLocalisation();

            //// Act
            var entry1 = localisationData.AddLanguage(language);
            var entry2 = localisationData.AddLanguage(language);

            //// Assert
            Assert.AreSame(entry1, entry2,
                "Adding a duplicate should return the existing object rather than a new one.");
            Assert.IsNull(localisationData.InternalVerifyState(), localisationData.InternalVerifyState());
        }


        [TestCase(new [] {"English"}, new [] { "Key1", "Key2"}, "Spanish" )]
        [TestCase(new[] { "English", "French" }, new[] { "Key1", "Key2", "Key3" }, "Spanish")]
        public void AddLanguageAdjustsEntries(string[] languages, string[] keys, string newLanguage)
        {
            // Arrange
            var localisationData = CreateNewLocalisation();

            foreach (var language in languages)
                localisationData.AddLanguage(language);

            foreach (var key in keys)
            {
                var entry = localisationData.AddEntry(key);
                for (var i = 0; i < languages.Length; i++)
                    entry.Languages[i] = key + "-" + languages[i];
            }

            //// Act
            localisationData.AddLanguage(newLanguage);

            //// Assert
            foreach (var key in keys)
            {
                var entry = localisationData.GetEntry(key);
                for (var i = 0; i < languages.Length; i++)
                    Assert.AreEqual(entry.Languages[i], key + "-" + languages[i], "Key is corrupted");
            }
            Assert.IsNull(localisationData.InternalVerifyState(), localisationData.InternalVerifyState());
        }

        [TestCase("English")]
        [TestCase("French")]
        public void GetLanguage(string language)
        {
            // Arrange
            var localisationData = CreateNewLocalisation();
            localisationData.AddLanguage(language);

            //// Act
            var entry = localisationData.GetLanguage(language);

            //// Assert
            Assert.AreEqual(language, entry.Name, "Language was not found");
            Assert.IsNull(localisationData.InternalVerifyState(), localisationData.InternalVerifyState());
        }


        [TestCase(new[] { "English" }, "French")]
        [TestCase(new[] { "English", "French" }, "English2")]
        [TestCase(new[] { "English", "French", "Spanish" }, "German")]
        public void GetLanguageNotFound(string[] languages, string checkLanguage)
        {
            //// Arrange
            var localisationData = CreateNewLocalisation();
            foreach (var language in languages)
                localisationData.AddLanguage(language);

            //// Act
            var isLanguageFound = localisationData.ContainsLanguage(checkLanguage);

            //// Assert
            Assert.IsFalse(isLanguageFound, "Language " + checkLanguage + " was found but should not be present");
            Assert.IsNull(localisationData.InternalVerifyState(), localisationData.InternalVerifyState());
        }


        [TestCase("English")]
        [TestCase("English", "French")]
        [TestCase("English", "French", "Spanish")]
        public void GetLanguages(params string[] languages)
        {
            //// Arrange
            var localisationData = CreateNewLocalisation();
            foreach (var language in languages)
                localisationData.AddLanguage(language);

            //// Act
            var returnedLanguages = localisationData.GetLanguages();

            //// Assert
            Assert.AreEqual(languages.Length, returnedLanguages.Count, "The number of languages is different");
            foreach (var language in languages)
                Assert.IsTrue(localisationData.ContainsLanguage(language), "Language " + language + " was not found at the correct index");
            Assert.IsNull(localisationData.InternalVerifyState(), localisationData.InternalVerifyState());
        }


        [TestCase("English")]
        [TestCase("English", "French")]
        [TestCase("English", "French", "Spanish")]
        public void GetLanguageIndex(params string[] languages)
        {
            //// Arrange
            var localisationData = CreateNewLocalisation();
            foreach(var language in languages)
                localisationData.AddLanguage(language);

            //// Assert
            for (int i = 0; i < languages.Length; i++)
                Assert.AreEqual(i, localisationData.GetLanguageIndex(languages[i]), "Language was not found at the correct index");
            Assert.IsNull(localisationData.InternalVerifyState(), localisationData.InternalVerifyState());
        }

        [TestCase("English")]
        [TestCase("French")]
        public void RemoveLanguage(string language)
        {
            // Arrange
            var localisationData = CreateNewLocalisation();
            localisationData.AddLanguage(language);

            //// Act
            localisationData.RemoveLanguage(language);

            //// Assert
            Assert.IsFalse(localisationData.ContainsLanguage(language), "Language was not removed");
                        Assert.IsNull(localisationData.InternalVerifyState(), localisationData.InternalVerifyState());
        }

        [TestCase("English", "English", "French", "Spanish")]
        [TestCase("German", "French", "German", "Spanish")]
        [TestCase("German", "French", "German", "English")]
        public void RemoveLanguageWhenMultiple(string languageToRemove, string language, string language2, string language3)
        {
            // Arrange
            var localisationData = CreateNewLocalisation();
            localisationData.AddLanguage(language);
            localisationData.AddLanguage(language2);
            localisationData.AddLanguage(language3);

            //// Act
            localisationData.RemoveLanguage(languageToRemove);

            //// Assert
            Assert.IsFalse(localisationData.ContainsLanguage(languageToRemove), "Language was not removed");
            Assert.IsNull(localisationData.InternalVerifyState(), localisationData.InternalVerifyState());
        }


        [TestCase(new[] { "English" }, new[] { "Key1", "Key2" }, "English")]
        [TestCase(new[] { "English2", "French" }, new[] { "Key1", "Key2", "Key3" }, "English2")]
        [TestCase(new[] { "English", "French" }, new[] { "Key1", "Key2", "Key3" }, "French")]
        [TestCase(new[] { "English3", "French", "Spanish" }, new[] { "Key1", "Key2", "Key3" }, "English3")]
        [TestCase(new[] { "English", "French3", "Spanish" }, new[] { "Key1", "Key2", "Key3" }, "French3")]
        [TestCase(new[] { "English", "French", "Spanish" }, new[] { "Key1", "Key2", "Key3" }, "Spanish")]
        public void RemoveLanguageAdjustsEntries(string[] languages, string[] keys, string removeLanguage)
        {
            // Arrange
            var localisationData = CreateNewLocalisation();

            foreach (var language in languages)
                localisationData.AddLanguage(language);

            foreach (var key in keys)
            {
                var entry = localisationData.AddEntry(key);
                for (var i = 0; i < languages.Length; i++)
                    entry.Languages[i] = key + "-" + languages[i];
            }

            //// Act
            localisationData.RemoveLanguage(removeLanguage);

            //// Assert
            foreach (var key in keys)
            {
                var entry = localisationData.GetEntry(key);
                for (int i = 0, newIndex = 0; i < languages.Length - 1; i++, newIndex++)
                {
                    if (languages[i] == removeLanguage) newIndex++; // skip one
                    Assert.AreEqual(entry.Languages[i], key + "-" + languages[newIndex],
                        "Key is corrupted when removing " + removeLanguage);
                }
            }
            Assert.IsNull(localisationData.InternalVerifyState(), localisationData.InternalVerifyState());
        }

        [TestCase("English")]
        [TestCase("German")]
        public void ContainsLanguage(string language)
        {
            // Arrange
            var localisationData = CreateNewLocalisation();
            localisationData.AddLanguage(language);

            //// Act
            var containslanguage = localisationData.ContainsLanguage(language);

            //// Assert
            Assert.IsTrue(containslanguage, "Language Was Not Found");
            Assert.IsNull(localisationData.InternalVerifyState(), localisationData.InternalVerifyState());
        }

        [TestCase("English", "German")]
        [TestCase("German", "French")]
        public void ContainsLanguageNotFound(string language, string languageToAdd)
        {
            // Arrange
            var localisationData = CreateNewLocalisation();
            localisationData.AddLanguage(languageToAdd);

            //// Act
            var containslanguage = localisationData.ContainsLanguage(language);

            //// Assert
            Assert.IsFalse(containslanguage, "Missing language was returned as found");
            Assert.IsNull(localisationData.InternalVerifyState(), localisationData.InternalVerifyState());
        }

        #endregion Language Tests

        #region LocalisationEntry Tests

        [TestCase("Key1")]
        [TestCase("Key2")]
        public void AddLocalisationEntry(string key)
        {
            // Arrange
            var localisationData = CreateNewLocalisation();

            //// Act
            localisationData.AddEntry(key);

            //// Assert
            Assert.IsTrue(localisationData.ContainsEntry(key), "LocalisationEntry not added");
            Assert.IsNull(localisationData.InternalVerifyState(), localisationData.InternalVerifyState());
        }

        [TestCase("Key1", "Key2", "Key3")]
        [TestCase("Key4", "Key5", "Key6")]
        public void AddLocalisationEntryMultiple(string key, string key2, string key3)
        {
            // Arrange
            var localisationData = CreateNewLocalisation();

            //// Act
            localisationData.AddEntry(key);
            localisationData.AddEntry(key2);
            localisationData.AddEntry(key3);

            //// Assert
            Assert.IsTrue(localisationData.ContainsEntry(key), "LocalisationEntry not added");
            Assert.IsTrue(localisationData.ContainsEntry(key2), "LocalisationEntry not added");
            Assert.IsTrue(localisationData.ContainsEntry(key3), "LocalisationEntry not added");
            Assert.IsNull(localisationData.InternalVerifyState(), localisationData.InternalVerifyState());
        }

        [TestCase("Key1")]
        [TestCase("Key2")]
        public void AddLocalisationEntryNoLanguagesCreatesDefault(string key)
        {
            // Arrange
            var localisationData = CreateNewLocalisation();

            //// Act
            localisationData.AddEntry(key);

            //// Assert
            Assert.IsTrue(localisationData.ContainsLanguage("English"), "Language not created");
            Assert.IsNull(localisationData.InternalVerifyState(), localisationData.InternalVerifyState());
        }

        [TestCase("Key1")]
        [TestCase("Key2")]
        public void AddLocalisationEntryNoDuplicates(string key)
        {
            // Arrange
            var localisationData = CreateNewLocalisation();

            //// Act
            var entry1 = localisationData.AddEntry(key);
            var entry2 = localisationData.AddEntry(key);

            //// Assert
            Assert.AreSame(entry1, entry2, "Adding a duplicate should return the existing object rather than a new one.");
            Assert.IsNull(localisationData.InternalVerifyState(), localisationData.InternalVerifyState());
        }


        [TestCase("Key1")]
        [TestCase("Key2")]
        public void GetLocalisationEntry(string key)
        {
            // Arrange
            var localisationData = CreateNewLocalisation();
            localisationData.AddEntry(key);

            //// Act
            var entry = localisationData.GetEntry(key);

            //// Assert
            Assert.AreEqual(key, entry.Key, "LocalisationEntry was not found");
            Assert.IsNull(localisationData.InternalVerifyState(), localisationData.InternalVerifyState());
        }

        [TestCase("Key1")]
        [TestCase("Key2")]
        public void RemoveLocalisationEntry(string key)
        {
            // Arrange
            var localisationData = CreateNewLocalisation();
            localisationData.AddEntry(key);

            //// Act
            localisationData.RemoveEntry(key);

            //// Assert
            Assert.IsFalse(localisationData.ContainsEntry(key), "LocalisationEntry was not removed");
            Assert.IsNull(localisationData.InternalVerifyState(), localisationData.InternalVerifyState());
        }

        [TestCase("English", "English", "French", "Spanish")]
        [TestCase("German", "French", "German", "Spanish")]
        [TestCase("German", "French", "German", "English")]
        public void RemoveLocalisationEntryWhenMultiple(string languageToRemove, string language, string language2, string language3)
        {
            // Arrange
            var localisationData = CreateNewLocalisation();
            localisationData.AddLanguage(language);
            localisationData.AddLanguage(language2);
            localisationData.AddLanguage(language3);

            //// Act
            localisationData.RemoveLanguage(languageToRemove);

            //// Assert
            Assert.IsFalse(localisationData.ContainsLanguage(languageToRemove), "Language was not removed");
            Assert.IsNull(localisationData.InternalVerifyState(), localisationData.InternalVerifyState());
        }

        [TestCase("Key1")]
        [TestCase("Key2")]
        public void ContainsLocalisationEntry(string key)
        {
            // Arrange
            var localisationData = CreateNewLocalisation();
            localisationData.AddEntry(key);

            //// Act
            var containsEntry = localisationData.ContainsEntry(key);

            //// Assert
            Assert.IsTrue(containsEntry, "LocalisationEntry Was Not Found");
            Assert.IsNull(localisationData.InternalVerifyState(), localisationData.InternalVerifyState());
        }

        [TestCase("Key1", "Key2")]
        [TestCase("Key2", "Key1")]
        public void ContainsLocalisationEntryNotFound(string key, string keyToAdd)
        {
            // Arrange
            var localisationData = CreateNewLocalisation();
            localisationData.AddEntry(keyToAdd);

            //// Act
            var containsEntry = localisationData.ContainsEntry(key);

            //// Assert
            Assert.IsFalse(containsEntry, "Missing LocalisationEntry was returned as found");
            Assert.IsNull(localisationData.InternalVerifyState(), localisationData.InternalVerifyState());
        }

        #endregion LocalisationEntry Tests		

        [Test]
        public void PlaceHolder()
        {
            // Arrange
            //var messenger = new Messenger();
            //_testHandlerCalled = false;
            //messenger.AddListener<BaseMessage>(TestHandler);

            //// Act
            //messenger.TriggerMessage(new BaseMessage());

            //// Assert
            //Assert.IsTrue(_testHandlerCalled, "The test handler was not called!");

            //// Cleanup
            //messenger.RemoveListener<BaseMessage>(TestHandler);
        }

    }
}
#endif