using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Media;
using System.Windows.Shapes;


namespace TheShortestPath
{    
    public partial class MainWindow : Window
    {
        private bool[,] DataFieldArray; 
        private PathFinder.Position StartPosition; 
        private PathFinder.Position EndPosition;
        private Button[,] buttons; 
        private List<PathFinder.Position> positions;
        private int obstacklescount;

        // This struct is for sending data to the button_click function
        // through tag property
         struct ButtonState
        {
           public bool State { get; set; }
           public int Iindex { get; set; }
           public int Jindex { get; set; }
                       
        }
      
        public MainWindow()
        {
            InitializeComponent();
        }


        private void СreateButton_Click(object sender, RoutedEventArgs e)
        {
            //initialization block
            positions = new List<PathFinder.Position>();
            StartPosition = new PathFinder.Position(-1,0);
            EndPosition = new PathFinder.Position(-1, 0);
            if (wrapPanel.Children.Count > 0)
                wrapPanel.Children.Clear();
            //Reading of the field properties
            int count = GetQuantityButtons();
            obstacklescount = GetQuantityObstackles();
            //Creating of the field, getting data to the DataFieldArray
            Button[,] buttons = CreateButtons(count);            
            AddToWrapPanel(buttons);
        }            
       
        private Button[,] CreateButtons(int quantity)
        {
            //init block
            ButtonState buttonstate= new ButtonState();
            buttons = new Button[quantity, quantity];
            DataFieldArray = new bool[quantity, quantity];
            //Creating Block
            for (int i = 0; i < quantity; i++)
                for (int j = 0; j < quantity; j++)
                { 
                        //set Button properties
                        buttons[i, j] = new Button();
                        buttons[i, j].Width = 40d;
                        buttons[i, j].Height = 40d;
                        buttons[i, j].Margin = new Thickness(1d);
                        //Depending on random true or false 
                        //custom properties are set
                        if (BoolRandomizer((i+1)*j) == true)
                        {
                            buttons[i, j].Background =
                                new LinearGradientBrush(Colors.LightBlue, Colors.MintCream, 90);
                            //Adding btn_click only for nonobsackles
                            buttons[i, j].Click += btn_click;
                            //Getting an information package about the button
                            buttonstate.Iindex = i;
                            buttonstate.Jindex = j;
                            buttonstate.State = false;
                            buttons[i, j].Tag = buttonstate;
                            //DataFieldArray  for further calculations
                            DataFieldArray[i, j] = true;
                        }
                        else
                        {
                            buttons[i, j].Background =
                                new LinearGradientBrush(Colors.Gray, Colors.MintCream, 90);
                            //DataFieldArray  for further calculations
                            DataFieldArray[i, j] = false;
                        }
                        buttons[i, j].Content = ((i*quantity)+(j+1)).ToString();                    
                }
            return buttons;
        }

        //Handles Item click and decides what kind of actions 
        //have to be occur
        private void btn_click(object sender, RoutedEventArgs e)
        {
            //Type Conversion
            Button button = (Button)sender;
            ButtonState buttonstate = (ButtonState)button.Tag; 
            //if button is unset it will get set, and if both are set then Getpath() 
            //will be called
                if (buttonstate.State == false)
                {
                    if (EndPosition.i == -1 || StartPosition.i == -1)
                    {
                        //setting set up
                        buttonstate.State = true;
                        button.Tag = buttonstate;
                        button.Background = new LinearGradientBrush(Colors.Green, Colors.MintCream, 90);
                        if (StartPosition.i == -1) 
                        {
                            StartPosition.i = buttonstate.Iindex; 
                            StartPosition.j = buttonstate.Jindex; 
                        }
                        else 
                        { 
                            EndPosition.i = buttonstate.Iindex;
                            EndPosition.j = buttonstate.Jindex; 
                        }
                        //Getpathcalling
                        if (EndPosition.i != -1 && StartPosition.i != -1) GetPath();
                    }
                }
            //otherwise it will get unset
                else
                {
                    //clear old path
                    if (positions.Count != 0) for (int i = 0; i <= positions.Count - 1; i++)
                            buttons[positions[i].i, positions[i].j].Background =
                                new LinearGradientBrush(Colors.LightBlue, Colors.MintCream, 90);
                    //unsetting set up
                    buttonstate.State = false;
                    button.Tag = buttonstate;
                    button.Background = new LinearGradientBrush(Colors.LightBlue, Colors.MintCream, 90);
                    //-1 in ButtonState index is a marker of unsetting
                    if (StartPosition.i == buttonstate.Iindex &&
                        StartPosition.j == buttonstate.Jindex) StartPosition.i = -1;
                    else EndPosition.i = -1;
                }
           
        }

        //Calls Pathfinder methods to get a path
        private void GetPath()
        {   
            //Creating an INstance and calling the public method Get() which returns
            // a list of coordinates
            PathFinder pathfinder = 
                new PathFinder(DataFieldArray, StartPosition, EndPosition);            
            positions = pathfinder.Get();
            // if method returned a list which has a one item contains (-1,0)
            //it means theres no way, so it call a messagebox
            if (positions.Contains(new PathFinder.Position(-1, 0))) 
            { 
                positions = new List<PathFinder.Position>(); 
                MessageBox.Show("Нельзя проложить путь."); 
            }
            //else we paint our path to the field
            else
            {
                for (int i = 0; i <= positions.Count - 1; i++)
                    buttons[positions[i].i, positions[i].j].Background =
                        new LinearGradientBrush(Colors.Yellow, Colors.MintCream, 90);
            }
        }

        //this function just adds created buttons
        private void AddToWrapPanel(Button[,] buttons)
        {
            for (int i = 0; i < (int)Math.Pow(buttons.Length, 0.5); i++)
                for (int j = 0; j < (int)Math.Pow(buttons.Length, 0.5); j++)
                wrapPanel.Children.Add(buttons[i,j]);
        }

        //Reading of the combobox1
        private int GetQuantityButtons()
        {
            ComboBoxItem item = (ComboBoxItem)comboBox1.SelectedItem;
            int count = int.Parse((string)item.Content);
            //setting the size of our field
            wrapPanel.Width = 40d * (count + 1) + count + 10d;
            return count;
            
        }

        //Reading of the combobox2
        private int GetQuantityObstackles()
        {
            ComboBoxItem item = (ComboBoxItem)comboBox2.SelectedItem;
            int count = int.Parse((string)item.Content);
            return count;
        }

           
        //This method returns true or false depending on a chance
        public bool BoolRandomizer(int i)
        {
            Random o_random = new Random((i + 1) * DateTime.Now.Millisecond*(int)DateTime.Now.Ticks);
            if (o_random.Next(1, 100) > obstacklescount)
                return true;
            else return false;
        }

        
    }


}
