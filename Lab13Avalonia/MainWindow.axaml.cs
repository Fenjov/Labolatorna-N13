using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using System;
using System.IO;

namespace Lab13Avalonia
{
    public partial class MainWindow : Window
    {
        private Bitmap? _spriteSheet;
        private DispatcherTimer _timer;
        
        // ВАЖЛИВО: Налаштуйте ці числа ПІД ВАШУ КАРТИНКУ sprite.jpg!
        // Наприклад, якщо на картинці 5 стовпців і 3 рядки:
        private int _columns = 5;       
        private int _rows = 3;          
        private int _totalFrames = 15;  // _columns * _rows (якщо в кінці є пусті місця, зменште це число, наприклад, 14)
        
        private int _currentFrame = 0;  
        private int _frameWidth;        
        private int _frameHeight;       

        public MainWindow()
        {
            InitializeComponent();
            InitializeSprite();
        }

        private void InitializeSprite()
        {
            try
            {
                // Завантажуємо зображення
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sprite.jpg");
                
                if (!File.Exists(path))
                {
                    ErrorText.Text = $"ПОМИЛКА: Файл не знайдено!\nПокладіть картинку 'sprite.jpg' за шляхом:\n{path}";
                    return;
                }

                _spriteSheet = new Bitmap(path);

                // АНАЛІЗ: Обчислюємо розміри одного кадру
                _frameWidth = (int)_spriteSheet.Size.Width / _columns;
                _frameHeight = (int)_spriteSheet.Size.Height / _rows;

                // Налаштовуємо таймер (оновлення кожні 80 мілісекунд для плавності)
                _timer = new DispatcherTimer();
                _timer.Interval = TimeSpan.FromMilliseconds(80);
                _timer.Tick += Timer_Tick;

                UpdateFrame();
            }
            catch (Exception ex)
            {
                ErrorText.Text = $"Помилка завантаження: {ex.Message}";
            }
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            // Переходимо до наступного кадру. 
            _currentFrame = (_currentFrame + 1) % _totalFrames;
            UpdateFrame();
        }

        private void UpdateFrame()
        {
            if (_spriteSheet == null) return;

            // ФОРМУЛИ ОБЧИСЛЕННЯ КООРДИНАТ КАДРУ
            int col = _currentFrame % _columns;
            int row = _currentFrame / _columns;

            int x = col * _frameWidth;
            int y = row * _frameHeight;

            // Перевірка, щоб не вийти за межі картинки (захист від помилок округлення)
            if (x + _frameWidth > _spriteSheet.Size.Width) _frameWidth = (int)_spriteSheet.Size.Width - x;
            if (y + _frameHeight > _spriteSheet.Size.Height) _frameHeight = (int)_spriteSheet.Size.Height - y;

            if (_frameWidth <= 0 || _frameHeight <= 0) return;

            PixelRect rect = new PixelRect(x, y, _frameWidth, _frameHeight);
            var croppedBitmap = new CroppedBitmap(_spriteSheet, rect);
            SpriteImage.Source = croppedBitmap;
        }

        public void ToggleAnimBtn_Click(object? sender, RoutedEventArgs e)
        {
            if (_timer == null) return;

            if (_timer.IsEnabled)
            {
                _timer.Stop();
            }
            else
            {
                _timer.Start();
            }
        }
    }
}