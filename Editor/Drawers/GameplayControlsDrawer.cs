﻿/// ------------------------------------------------
/// <summary>
/// Gameplay Controls Drawer
/// Purpose: 	Draws the gameplay controls.
/// Author:		Juan Silva
/// Date: 		November 28, 2015
/// Copyright (c) Tuxedo Berries All rights reserved.
/// </summary>
/// ------------------------------------------------
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using TuxedoBerries.ScenePanel.Constants;

namespace TuxedoBerries.ScenePanel.Drawers
{
	public class GameplayControlsDrawer
	{
		private ColorStack _colorStack;
		private TextureDatabaseProvider _textureProvider;
		private ISceneEntity _firstScene;
		private GUIContentCache _contentCache;
		private bool _hittedPlay = false;
		private bool _performStep = false;

		public GameplayControlsDrawer ()
		{
			_hittedPlay = false;
			_colorStack = new ColorStack ();
			_textureProvider = new TextureDatabaseProvider ();
			_contentCache = new GUIContentCache ();
		}

		/// <summary>
		/// Updates the first scene.
		/// </summary>
		/// <param name="entity">Entity.</param>
		public void UpdateFirstScene(ISceneEntity entity)
		{
			_firstScene = entity;
		}

		/// <summary>
		/// Raises the inspector update event.
		/// </summary>
		public void OnInspectorUpdate()
		{
			if (_performStep) {
				EditorApplication.Step ();
				EditorApplication.isPaused = true;
				_performStep = false;
			}
		}

		/// <summary>
		/// Draws the general controls.
		/// - Play from start
		/// - Play
		/// - Pause
		/// - Stop
		/// </summary>
		public void DrawGeneralControls()
		{
			EditorGUILayout.BeginHorizontal ();
			{
				// Play From Start
				_colorStack.Push ( GetPlayFromStartButtonColor() );
				if (GUILayout.Button ( GetContent(IconSet.PLAY_START_ICON, TooltipSet.PLAY_START_TOOLTIP) )) {
					if (!IsPlaying && SceneMainPanelUtility.OpenScene (_firstScene)) {
						_hittedPlay = true;
						EditorApplication.isPlaying = true;
					}
				}
				_colorStack.Pop ();


				if (!IsPlaying) {
					// Play Current
					_colorStack.Push ( GetPlayButtonColor() );
					if (GUILayout.Button (GetContent(IconSet.PLAY_ICON, TooltipSet.PLAY_TOOLTIP))) {
						_hittedPlay = true;
						EditorApplication.isPlaying = true;
					}
					_colorStack.Pop ();
				} else {
					// Stop
					_colorStack.Push ( GetStopButtonColor() );
					if (GUILayout.Button ( GetContent(IconSet.STOP_ICON, TooltipSet.STOP_TOOLTIP) )) {
						EditorApplication.isPlaying = false;
					}
					_colorStack.Pop ();
				}

				// Pause
				_colorStack.Push ( GetPauseButtonColor() );
				if (GUILayout.Button ( GetContent(IconSet.PAUSE_ICON, TooltipSet.PAUSE_TOOLTIP))) {
					EditorApplication.isPaused = !EditorApplication.isPaused;
				}
				_colorStack.Pop ();

				// Step
				_colorStack.Push ( GetStepButtonColor() );
				if (GUILayout.Button ( GetContent(IconSet.STEP_ICON, TooltipSet.STEP_TOOLTIP))) {
					if(IsPlaying)
						_performStep = true;
				}
				_colorStack.Pop ();
			}
			EditorGUILayout.EndHorizontal ();
		}

		/// <summary>
		/// Gets a value indicating whether the editor is in play mode.
		/// </summary>
		/// <value><c>true</c> if the game is playing; otherwise, <c>false</c>.</value>
		public bool IsPlaying {
			get {
				return _hittedPlay || EditorApplication.isPlaying || Application.isPlaying;
			}
		}

		#region helpers
		private GUIContent GetContent(string texture, string tooltip)
		{
			if (!_contentCache.Contains (texture)) {
				_contentCache [texture] = new GUIContent (_textureProvider.GetRelativeTexture (texture), tooltip);
			}

			return _contentCache [texture];
		}

		private Color GetPlayFromStartButtonColor()
		{
			return !IsPlaying ? ColorPalette.PlayButton_ON : ColorPalette.PlayButton_OFF;
		}

		private Color GetPlayButtonColor()
		{
			return !IsPlaying ? ColorPalette.PlayButton_ON : ColorPalette.PlayButton_OFF;
		}

		private Color GetPauseButtonColor()
		{
			if (!IsPlaying)
				return ColorPalette.PauseButton_OFF;
			
			return EditorApplication.isPaused ? ColorPalette.PauseButton_HOLD : ColorPalette.PauseButton_ON;
		}

		private Color GetStopButtonColor()
		{
			return IsPlaying ? ColorPalette.StopButton_ON : ColorPalette.StopButton_OFF;
		}

		private Color GetStepButtonColor()
		{
			return IsPlaying ? ColorPalette.StepButton_ON : ColorPalette.StepButton_OFF;
		}
		#endregion
	}
}
