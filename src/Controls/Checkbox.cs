﻿using System;
using System.Collections.Generic;
using Xamarin.Forms;
using SkiaSharp.Views.Forms;
using SkiaSharp;
using System.ComponentModel;
using System.Windows.Input;

namespace IntelliAbb.Xamarin.Controls
{
    /// <summary>
    /// This is a cross-platform checkbox control.
    /// </summary>
    [Browsable(true)]
    public class Checkbox : ContentView, IDisposable
    {
        #region Fields

        const double DEFAULT_SIZE = 28;
        bool _isAnimating;
        SKCanvasView _skiaView;
        ICommand _toggleCommand;

        #endregion

        #region Constructor

        public Checkbox()
        {
            InitializeCanvas();
            Content = _skiaView;
            HorizontalOptions = VerticalOptions = new LayoutOptions(LayoutAlignment.Center, false);
        }

        #endregion

        #region Initialize Canvas

        void InitializeCanvas()
        {
            _toggleCommand = new Command(OnTappedCommand);

            _skiaView = new SKCanvasView();
            _skiaView.PaintSurface += Handle_PaintSurface;
            _skiaView.WidthRequest = _skiaView.HeightRequest = DEFAULT_SIZE;
            _skiaView.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = _toggleCommand
            });
        }

        async void OnTappedCommand(object obj)
        {
            if (_isAnimating)
                return;

            _isAnimating = true;
            IsChecked = !IsChecked;

            await _skiaView.ScaleTo(0.85, 100);
            _skiaView.InvalidateSurface();
            await _skiaView.ScaleTo(1, 100, IsChecked ? Easing.BounceOut : null);
            _isAnimating = false;
        }
        #endregion

        #region Checkmark Paint Surface
        void Handle_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            e?.Surface?.Canvas?.Clear();

            DrawOutline(e);

            if (IsChecked)
                DrawCheckFilled(e);
        }

        void DrawCheckFilled(SKPaintSurfaceEventArgs e)
        {
            var imageInfo = e.Info;
            var canvas = e?.Surface?.Canvas;

            using (var checkfill = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = FillColor.ToSKColor(),
                StrokeJoin = SKStrokeJoin.Round,
                IsAntialias = true
            })
            {
                if (Shape == Shape.Circle)
                    canvas.DrawCircle(imageInfo.Width / 2, imageInfo.Height / 2, (imageInfo.Width / 2) - (OutlineWidth / 2), checkfill);
                else
                    canvas.DrawRoundRect(OutlineWidth, OutlineWidth, imageInfo.Width - (OutlineWidth * 2), imageInfo.Height - (OutlineWidth * 2), OutlineWidth, OutlineWidth, checkfill);
            }

            using (var checkPath = new SKPath())
            {
                checkPath.MoveTo(.275f * imageInfo.Width, .5f * imageInfo.Height);
                checkPath.LineTo(.425f * imageInfo.Width, .65f * imageInfo.Height);
                checkPath.LineTo(.725f * imageInfo.Width, .375f * imageInfo.Height);

                using (var checkStroke = new SKPaint
                {
                    Style = SKPaintStyle.Stroke,
                    Color = CheckColor.ToSKColor(),
                    StrokeWidth = (float)OutlineWidth,
                    StrokeCap = SKStrokeCap.Round,
                    IsAntialias = true
                })
                {
                    canvas.DrawPath(checkPath, checkStroke);
                }
            }
        }

        void DrawOutline(SKPaintSurfaceEventArgs e)
        {

            var imageInfo = e.Info;
            var canvas = e?.Surface?.Canvas;

            using (var outline = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = OutlineColor.ToSKColor(),
                StrokeWidth = OutlineWidth,
                StrokeJoin = SKStrokeJoin.Round,
                IsAntialias = true
            })
            {
                if (Shape == Shape.Circle)
                    canvas.DrawCircle(imageInfo.Width / 2, imageInfo.Height / 2, (float)((imageInfo.Width / 2) - (OutlineWidth / 2)), outline);
                else
                    canvas.DrawRoundRect(OutlineWidth, OutlineWidth, imageInfo.Width - (OutlineWidth * 2), imageInfo.Height - (OutlineWidth * 2), OutlineWidth, OutlineWidth, outline);
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Raised when IsChecked is changed.
        /// </summary>
        public event EventHandler<TappedEventArgs> IsCheckedChanged;
        #endregion

        #region Bindable Properties
        public static BindableProperty OutlineColorProperty = BindableProperty.Create(nameof(OutlineColor), typeof(Color), typeof(Checkbox), Color.Blue);

        /// <summary>
        /// Gets or sets the color of the outline.
        /// </summary>
        /// <value>Xamarin.Forms.Color value of the outline</value>
        public Color OutlineColor
        {
            get { return (Color)GetValue(OutlineColorProperty); }
            set { SetValue(OutlineColorProperty, value); }
        }

        public static BindableProperty FillColorProperty = BindableProperty.Create(nameof(FillColor), typeof(Color), typeof(Checkbox), Color.Blue);
        /// <summary>
        /// Gets or sets the color of the fill.
        /// </summary>
        /// <value>Xamarin.Forms.Color value of the fill.</value>
        public Color FillColor
        {
            get { return (Color)GetValue(FillColorProperty); }
            set { SetValue(FillColorProperty, value); }
        }

        public static BindableProperty CheckColorProperty = BindableProperty.Create(nameof(CheckColor), typeof(Color), typeof(Checkbox), Color.White);
        /// <summary>
        /// Gets or sets the color of the check.
        /// </summary>
        /// <value>Xamarin.Forms.Color value of the check.</value>
        public Color CheckColor
        {
            get { return (Color)GetValue(CheckColorProperty); }
            set { SetValue(CheckColorProperty, value); }
        }

        public static BindableProperty OutlineWidthProperty = BindableProperty.Create(nameof(OutlineWidth), typeof(float), typeof(Checkbox), 8.0f);
        /// <summary>
        /// Gets or sets the width of the outline and check.
        /// </summary>
        /// <value>The width of the outline and check.</value>
        public float OutlineWidth
        {
            get { return (float)GetValue(OutlineWidthProperty); }
            set { SetValue(OutlineWidthProperty, value); }
        }

        public static BindableProperty ShapeProperty = BindableProperty.Create(nameof(Shape), typeof(Shape), typeof(Checkbox), Shape.Circle);
        /// <summary>
        /// Gets or sets the shape of the <see cref="T:IntelliAbb.Xamarin.Controls.Checkbox"/>.
        /// </summary>
        public Shape Shape
        {
            get { return (Shape)GetValue(ShapeProperty); }
            set { SetValue(ShapeProperty, value); }
        }

        public static new BindableProperty StyleProperty = BindableProperty.Create(nameof(Style), typeof(Style), typeof(Checkbox), propertyChanged: OnStyleChanged);

        /// <summary>
        /// Gets or sets the style for <see cref="T:IntelliAbb.Xamarin.Controls.Checkbox"/>.
        /// </summary>
        /// <value>The style.</value>
        public new Style Style
        {
            get { return (Style)GetValue(StyleProperty); }
            set { SetValue(StyleProperty, value); }
        }

        static void OnStyleChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (!(bindable is Checkbox checkbox)) return;

            var setters = ((Style)newValue).Setters;
            var dict = new Dictionary<string, Color>();

            foreach (var setter in setters)
            {
                dict.Add(setter.Property.PropertyName, (Color)setter.Value);
            }

            checkbox.OutlineColor = dict[nameof(OutlineColor)];
            checkbox.FillColor = dict[nameof(FillColor)];
            checkbox.CheckColor = dict[nameof(CheckColor)];
        }

        public static readonly BindableProperty IsCheckedProperty = BindableProperty.Create(nameof(IsChecked), typeof(bool), typeof(Checkbox), false, BindingMode.TwoWay, propertyChanged: OnIsCheckedChanged);
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:IntelliAbb.Xamarin.Controls.Checkbox"/> is checked.
        /// </summary>
        /// <value><c>true</c> if is checked; otherwise, <c>false</c>.</value>
        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        static void OnIsCheckedChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var checkbox = bindable as Checkbox;
            checkbox?.IsCheckedChanged?.Invoke(checkbox, new TappedEventArgs((bool)newValue));
            checkbox?._skiaView?.InvalidateSurface();
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            _skiaView.PaintSurface -= Handle_PaintSurface;
        }

        #endregion
    }
    public enum Shape
    {
        Circle,
        Rectangle
    }
}

