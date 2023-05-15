using Microsoft.Win32;
using System;
using System.IO;
using System.IO.Pipes;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace OOP22
{
    public partial class MainWindow : Window
    {
        public static readonly RoutedUICommand LoadImageCommand = new RoutedUICommand("Load Image", "LoadImageCommand", typeof(MainWindow));
        public static RoutedCommand AlignLeftCommand = new RoutedCommand();
        public static RoutedCommand AlignCenterCommand = new RoutedCommand();
        public static RoutedCommand AlignRightCommand = new RoutedCommand();
        public MainWindow()
        {
            InitializeComponent();
            CommandBinding openCommandBinding = new CommandBinding(ApplicationCommands.Open, OpenCommandExecuted);
            CommandBinding saveCommandBinding = new CommandBinding(ApplicationCommands.Save, SaveCommandExecuted);

            CommandBindings.Add(openCommandBinding);
            CommandBindings.Add(saveCommandBinding);
            CommandBindings.Add(new CommandBinding(LoadImageCommand, LoadImageCommandExecuted));
            CommandBinding alignLeftBinding = new CommandBinding(AlignLeftCommand, AlignLeftCommandExecuted);
            CommandBinding alignCenterBinding = new CommandBinding(AlignCenterCommand, AlignCenterCommandExecuted);
            CommandBinding alignRightBinding = new CommandBinding(AlignRightCommand, AlignRightCommandExecuted);

            this.CommandBindings.Add(alignLeftBinding);
            this.CommandBindings.Add(alignCenterBinding);
            this.CommandBindings.Add(alignRightBinding);
        }
        private void OpenCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                currentFilePath = openFileDialog.FileName;
                rtbEditor.Document.Blocks.Clear();
                string fileContent = File.ReadAllText(currentFilePath);
                rtbEditor.Document.Blocks.Add(new Paragraph(new Run(fileContent)));
                this.Title = Path.GetFileName(currentFilePath);
            }
        }
        private string currentFilePath;
        private void SaveCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text Files (*.txt)|*.txt|Rich Text Format (*.rtf)|*.rtf|All Files (*.*)|*.*";

            if (!string.IsNullOrEmpty(currentFilePath))
            {
                saveFileDialog.FileName = Path.GetFileName(currentFilePath);
            }
            else if (!string.IsNullOrEmpty(this.Title) && this.Title != "MainWindow")
            {
                saveFileDialog.FileName = this.Title;
            }
            else
            {
                saveFileDialog.FileName = "Untitled";
            }

            if (saveFileDialog.ShowDialog() == true)
            {
                currentFilePath = saveFileDialog.FileName;

                using (FileStream fs = new FileStream(currentFilePath, FileMode.Create))
                {
                    TextRange textRange = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
                    if (saveFileDialog.FilterIndex == 1)
                    {
                        textRange.Save(fs, DataFormats.Text);
                    }
                    else if (saveFileDialog.FilterIndex == 2)
                    {
                        textRange.Save(fs, DataFormats.Rtf);
                    }
                }
            }
        }

        private void cmbFontFamily_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            ComboBoxItem selectedItem = (ComboBoxItem)comboBox.SelectedItem;

            if (selectedItem != null)
            {
                string selectedFont = selectedItem.Content.ToString();
                rtbEditor.Selection.ApplyPropertyValue(TextElement.FontFamilyProperty, new FontFamily(selectedFont));
            }
        }

        private void cmbFontSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            ComboBoxItem selectedItem = (ComboBoxItem)comboBox.SelectedItem;

            if (selectedItem != null)
            {
                string selectedFontSize = selectedItem.Content.ToString();

                if (double.TryParse(selectedFontSize, out double fontSize))
                {
                    rtbEditor.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, fontSize);
                }
            }
        }

        private void rtbEditor_SelectionChanged(object sender, RoutedEventArgs e)
        {
            object fontWeight = rtbEditor.Selection.GetPropertyValue(TextElement.FontWeightProperty);
            btnBold.IsChecked = (fontWeight != DependencyProperty.UnsetValue) && (fontWeight.Equals(FontWeights.Bold));

            object fontStyle = rtbEditor.Selection.GetPropertyValue(TextElement.FontStyleProperty);
            btnItalic.IsChecked = (fontStyle != DependencyProperty.UnsetValue) && (fontStyle.Equals(FontStyles.Italic));

            object textDecoration = rtbEditor.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
            btnUnderline.IsChecked = (textDecoration != DependencyProperty.UnsetValue) && (textDecoration.Equals(TextDecorations.Underline));
        }
        private void LoadImageCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files (*.png;*.jpg;*.jpeg;*.gif;*.bmp)|*.png;*.jpg;*.jpeg;*.gif;*.bmp|All Files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                string imagePath = openFileDialog.FileName;
                Image image = new Image();
                BitmapImage bitmapImage = new BitmapImage(new Uri(imagePath));
                image.Source = bitmapImage;
                image.Width = 200;
                image.Height = 200;

                if (rtbEditor.CaretPosition != null)
                {
                    InlineUIContainer container = new InlineUIContainer(image, rtbEditor.CaretPosition);
                }
            }
        }
        private void AlignLeftCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (rtbEditor != null)
            {
                if (rtbEditor.Selection.GetPropertyValue(Paragraph.TextAlignmentProperty) is TextAlignment alignment && alignment != TextAlignment.Left)
                {
                    rtbEditor.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, TextAlignment.Left);
                }
            }
        }
        private void AlignCenterCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (rtbEditor != null)
            {
                if (rtbEditor.Selection.GetPropertyValue(Paragraph.TextAlignmentProperty) is TextAlignment alignment && alignment != TextAlignment.Center)
                {
                    rtbEditor.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, TextAlignment.Center);
                }
            }
        }
        private void AlignRightCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (rtbEditor != null)
            {
                if (rtbEditor.Selection.GetPropertyValue(Paragraph.TextAlignmentProperty) is TextAlignment alignment && alignment != TextAlignment.Right)
                {
                    rtbEditor.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, TextAlignment.Right);
                }
            }
        }
    }
}
