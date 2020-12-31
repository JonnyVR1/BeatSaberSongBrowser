﻿using System.Collections;
using System.Collections.Concurrent;
using HMUI;
using SongBrowser.Internals;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SongBrowser.UI
{
    /// <summary>
    /// Taken from SongCore, modified
    /// </summary>
    public class ProgressBar : MonoBehaviour
    {
        private Canvas _canvas;
        private TMP_Text _authorNameText;
        private TMP_Text _pluginNameText;
        private TMP_Text _headerText;
        internal Image _loadingBackg;

        private static readonly Vector3 Position = new Vector3(0, 0.0f, 2.6f);
        private static readonly Vector3 Rotation = Vector3.zero;
        private static readonly Vector3 Scale = new Vector3(0.01f, 0.01f, 0.01f);

        private static readonly Vector2 CanvasSize = new Vector2(200, 50);

        private const string AuthorNameText = "Halsafar";
        private const float AuthorNameFontSize = 7f;
        private static readonly Vector2 AuthorNamePosition = new Vector2(10, 31);

        private const string PluginNameText = "Song Browser - v<size=100%>" + Plugin.VERSION_NUMBER + "</size>";
        private const float PluginNameFontSize = 9f;
        private static readonly Vector2 PluginNamePosition = new Vector2(10, 23);

        private static readonly Vector2 HeaderPosition = new Vector2(10, 15);
        private static readonly Vector2 HeaderSize = new Vector2(100, 20);
        private const string HeaderText = "Processing songs...";
        private const float HeaderFontSize = 15f;

        private static readonly Vector2 LoadingBarSize = new Vector2(100, 10);
        private static readonly Color BackgroundColor = new Color(0, 0, 0, 0.2f);

        private bool _showingMessage;

        public static ProgressBar Create()
        {
            return new GameObject("SongBrowserLoadingStatus").AddComponent<ProgressBar>();
        }

        public void ShowMessage(string message, float time)
        {
            StopAllCoroutines();
            _showingMessage = true;
            _headerText.text = message;
            _loadingBackg.enabled = false;
            gameObject.SetActive(true);
            StartCoroutine(DisableCanvasRoutine(time));
        }

        public void ShowMessage(string message)
        {
            StopAllCoroutines();
            _showingMessage = true;
            _headerText.text = message;
            _loadingBackg.enabled = false;
            gameObject.SetActive(true);
        }

        private void OnEnable()
        {
            SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;
            SongBrowserModel.didFinishProcessingSongs += SongBrowserFinishedProcessingSongs;
        }

        private void OnDisable()
        {
            SceneManager.activeSceneChanged -= SceneManagerOnActiveSceneChanged;
            SongBrowserModel.didFinishProcessingSongs -= SongBrowserFinishedProcessingSongs;
        }

        private void SceneManagerOnActiveSceneChanged(Scene oldScene, Scene newScene)
        {
            if (newScene.name == "MenuCore")
            {
                if (_showingMessage)
                {
                    gameObject.SetActive(true);
                }
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        private void SongBrowserFinishedProcessingSongs(ConcurrentDictionary<string, CustomPreviewBeatmapLevel> arg2)
        {
            StopAllCoroutines();
            _showingMessage = false;
            _headerText.text = arg2.Count + " songs processed";
            _loadingBackg.enabled = false;
            StartCoroutine(DisableCanvasRoutine(20f));
        }

        private IEnumerator DisableCanvasRoutine(float time)
        {
            yield return new WaitForSecondsRealtime(time);
            _showingMessage = false;
            gameObject.SetActive(false);
        }

        private void Awake()
        {
            _canvas = gameObject.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.WorldSpace;
            
            gameObject.AddComponent<CurvedCanvasSettings>().SetRadius(0f);

            var ct = _canvas.transform;
            ct.position = Position;
            ct.localScale = Scale;

            if (ct is RectTransform crt)
            {
                crt.sizeDelta = CanvasSize;

                _authorNameText = BeatSaberUI.CreateText(crt, AuthorNameText, AuthorNameFontSize, AuthorNamePosition, HeaderSize);

                _pluginNameText = BeatSaberUI.CreateText(crt, PluginNameText, PluginNameFontSize, PluginNamePosition, HeaderSize);

                _headerText = BeatSaberUI.CreateText(crt, HeaderText, HeaderFontSize, HeaderPosition, HeaderSize);
            }

            _loadingBackg = new GameObject("Background").AddComponent<ImageView>();
            if (_loadingBackg.transform is RectTransform lbrt)
            {
                lbrt.SetParent(_canvas.transform, false);
                lbrt.sizeDelta = LoadingBarSize;
            }

            _loadingBackg.color = BackgroundColor;

            gameObject.SetActive(false);
        }

        private void Update()
        {
            if (!_canvas.enabled) return;
        }
    }
}
