﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Blish_HUD.Controls;
using Microsoft.Scripting.Utils;
using Microsoft.Xna.Framework;

namespace Blish_HUD.Settings.UI.Views {

    public class EnumSettingView<TEnum> : SettingView<TEnum> where TEnum : struct, Enum {

        private const int CONTROL_PADDING = 5;

        private const int DROPDOWN_WIDTH  = 250;
        private const int DROPDOWN_HEIGHT = 27;

        private Label    _displayNameLabel;
        private Dropdown _enumDropdown;

        private TEnum[] _enumValues;

        public EnumSettingView(SettingEntry<TEnum> setting, int definedWidth = -1) : base(setting, definedWidth) { /* NOOP */ }

        protected override Task<bool> Load(IProgress<string> progress) {
            progress.Report("Loading setting values...");
            _enumValues = Enum.GetValues(typeof(TEnum)).Cast<TEnum>().ToArray();
            progress.Report(string.Empty);

            return base.Load(progress);
        }

        protected override void BuildSetting(Panel buildPanel) {
            _displayNameLabel = new Label() {
                AutoSizeWidth = true,
                Location      = new Point(CONTROL_PADDING, 0),
                Parent        = buildPanel
            };

            _enumDropdown = new Dropdown() {
                Size   = new Point(DROPDOWN_WIDTH, DROPDOWN_HEIGHT),
                Parent = buildPanel
            };

            _enumDropdown.Items.AddRange(_enumValues.Select(e => e.ToString()));

            _enumDropdown.ValueChanged += EnumDropdownOnValueChanged;
        }

        private void EnumDropdownOnValueChanged(object sender, ValueChangedEventArgs e) {
            if (Enum.TryParse(e.CurrentValue, true, out TEnum value)) {
                this.OnValueChanged(new ValueEventArgs<TEnum>(value));
            } else {
                _enumDropdown.SelectedItem = this.Value.ToString();
            }
        }

        private void UpdateSizeAndLayout() {
            this.ViewTarget.Height   = _enumDropdown.Bottom;
            _displayNameLabel.Height = this.ViewTarget.Height;

            if (this.DefinedWidth > 0) {
                _enumDropdown.Left    = _displayNameLabel.Right + CONTROL_PADDING;
                this.ViewTarget.Width = _enumDropdown.Right     + CONTROL_PADDING;
            } else {
                _enumDropdown.Location = new Point(this.ViewTarget.Width - CONTROL_PADDING - DROPDOWN_WIDTH, 0);
            }
        }

        protected override void RefreshDisplayName(string displayName) {
            _displayNameLabel.Text = displayName;

            UpdateSizeAndLayout();
        }

        protected override void RefreshDescription(string description) {
            _enumDropdown.BasicTooltipText = description;
        }

        protected override void RefreshValue(TEnum value) {
            _enumDropdown.SelectedItem = value.ToString();
        }

        protected override void Unload() {
            _enumDropdown.ValueChanged -= EnumDropdownOnValueChanged;
        }

    }
}
