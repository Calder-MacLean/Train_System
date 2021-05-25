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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using Business_Objects;
using Data_Layer;


/*
 * Author: Calder MacLean Matric No:  40313018
 * This program will allow for trains to be added the the service and for passengers to be added to the different trains
 * Modification Date 05/12/18
*/

namespace Coursework_40313018
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private trainList trains = new trainList();
        private passengerList passengers = new passengerList();
        private DateTime departTime;
        public int seatNo;
        public bool validDepartureTime = true;
        public int ticketCost;
        public bool validCabin = true;
        public bool validStops = true;
        public int errors = 0;
        bool validCoach;
        public MainWindow()
        {
            InitializeComponent();
            lblIntervalStations.Visibility = Visibility.Hidden;
            checkBoxNewcastle.Visibility = Visibility.Hidden;
            checkBoxDarlington.Visibility = Visibility.Hidden;
            checkBoxYork.Visibility = Visibility.Hidden;
            checkBoxPeterborough.Visibility = Visibility.Hidden;
            trains.readFromFile();
            passengers.readFromFile();
            listBoxTrainsAdded.Items.Clear();
            foreach (Train t in trains.GetTrains)
            {
                comboBoxPassengerTrainID.Items.Add(t.ID);
                listBoxTrainsAdded.Items.Add(t.ToString());
            }
            if (comboBoxTrainType.SelectedIndex == -1)
            {
                btnAddTrain.IsEnabled = false;
            }
            if(comboBoxPassengerSeat.SelectedIndex < 0)
            {
                btnAddPassenger.IsEnabled = false;
            }
        }

        private void btnAddTrain_Click(object sender, RoutedEventArgs e)
        {
            validCabin = true;
            TrainFactory tf = new TrainFactory();
            var newTrain = tf.BuildTrain(comboBoxTrainType.Text);
            newTrain.ID = txtboxTrainID.Text.ToUpper();
            newTrain.type = comboBoxTrainType.Text;
            newTrain.departure = comboBoxTrainDepartureStation.Text;
            newTrain.destination = comboBoxTrainArrivalStation.Text;
            newTrain.time = comboBoxTrainDepartureTime.Text;
            newTrain.day = datepickerTrainDepartureDate.Text;

            //if statment to see if train is sleeper or stopper and then add intermediate stops
            if (newTrain.type == "Sleeper" || newTrain.type == "Stopper")
            {
                if (checkBoxNewcastle.IsChecked == true)
                {
                    newTrain.stops = newTrain.stops + "Newcastle ";
                }
                if (checkBoxDarlington.IsChecked == true)
                {
                    newTrain.stops = newTrain.stops + "Darlington ";
                }
                if (checkBoxYork.IsChecked == true)
                {
                    newTrain.stops = newTrain.stops + "York ";
                }
                if (checkBoxPeterborough.IsChecked == true)
                {
                    newTrain.stops = newTrain.stops + "Peterborough ";
                }
            }

            //if statemnt to check if train has cabin or not
            if (comboBoxCabin.SelectedIndex == 0)
            {
                newTrain.sleeper = true;
            }
            else if (comboBoxCabin.SelectedIndex == 1)
            {
                newTrain.sleeper = false;
            }
            else if(comboBoxCabin.SelectedIndex == -1)
            {
                MessageBox.Show("Must Select Yes or No", "Cabin");
            }

            //if statemnt to check if train has firstclass or not
            if (comboBoxTrainFirstClass.SelectedIndex == 0)
            {
                newTrain.firstClass = true;
            }
            else if (comboBoxTrainFirstClass.SelectedIndex == 1)
            {
                newTrain.firstClass = false;
            }
            else
            {
                MessageBox.Show("Must Select Yes or No", "First Class");
            }

            //try catch to make sure a train type has been selected
            try
            {
                if (comboBoxTrainType.SelectedIndex == -1)
                {
                    throw new System.ArgumentException("Must Select a type of Train", "Train Type");
                }
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
            }

            //try catch that checks if the train ID entered is valid
            try
            {
                if (trains.validID(txtboxTrainID.Text) == false)
                {
                    throw new System.ArgumentException("Must enter a valid Train ID", "Train ID");
                }
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
            }

            //try catch to check if the train is a sleeper and it has a valid departure time
            try
            {
                if(comboBoxTrainDepartureTime.SelectedIndex != -1)
                {
                    departTime = Convert.ToDateTime(comboBoxTrainDepartureTime.Text);
                    if (departTime.Hour < 21 && comboBoxTrainType.SelectedIndex == 1)
                    {
                        throw new System.ArgumentException("Invalid Departure time for train", "Departure Time");
                    }
                }
                
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
                validDepartureTime = false;
            }

            //try catch to check if a date has been entered
            try
            {
                if (datepickerTrainDepartureDate.Text == "")
                {
                    throw new System.ArgumentException("Invalid Departure Date", "Departure Date");
                }
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
            }

            //try catch to make sure a departure time as been selected
            try
            {
                if (comboBoxTrainDepartureTime.SelectedIndex == -1)
                {
                    throw new System.ArgumentException("Invalid Departure time for train", "Departure Time");
                }
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
            }

            //try catch to make sure train ID has a value
            try
            {
                if (txtboxTrainID.Text == "")
                {
                    throw new System.ArgumentException("Invalid Train ID", "Train ID");
                }
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
            }

            //try catch to make sure the departure station and arrival destination are valid
            try
            {
                if (newTrain.validDepartureAndArrival(newTrain.departure, newTrain.destination) == false)
                {
                    throw new System.ArgumentException("Must enter a valid deparutre and arrival", "Departure Station, Arrival Station");
                }
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
            }

            //try catch that makes sure train ID has valid characters
            try
            {
                if (newTrain.validCharacters(txtboxTrainID.Text) == false)
                {
                    throw new System.ArgumentException("Must enter a valid ID", "Train ID");
                }
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
            }

            //try catch that makes sure departure and arrival station have a value 
            try
            {
                if (comboBoxTrainDepartureStation.SelectedIndex == -1 || comboBoxTrainArrivalStation.SelectedIndex == -1)
                {
                    throw new System.ArgumentException("Must enter a Departure Station", "Departure Station");
                }
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
            }

            //try catch to make sure sleeper train has a cabin
            try
            {
                if (comboBoxCabin.SelectedIndex == 0 && comboBoxTrainType.Text != "Sleeper" || comboBoxCabin.SelectedIndex == 1 && comboBoxTrainType.Text == "Sleeper")
                {
                    throw new System.ArgumentException("Train must be a sleeper to have a cabin", "Cabin");
                }
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
                validCabin = false;
            }

            //if statement that checks if everything is correct then the train is added to the train list
            if (trains.validID(newTrain.ID) && newTrain.validCharacters(newTrain.ID) && newTrain.validDepartureAndArrival(newTrain.departure, newTrain.destination) && comboBoxTrainDepartureStation.SelectedIndex != -1 && comboBoxTrainArrivalStation.SelectedIndex != -1 && comboBoxTrainDepartureTime.SelectedIndex != -1 && datepickerTrainDepartureDate.Text != "" && comboBoxTrainFirstClass.SelectedIndex != -1 && comboBoxCabin.SelectedIndex != -1 && validCabin && validDepartureTime)
            {
                trains.addTrain(newTrain);
                //trains.WriteToFile();
                comboBoxPassengerTrainID.Items.Add(newTrain.ID);
                listBoxTrainsAdded.Items.Add(newTrain.ToString());
                MessageBox.Show("Train Added");
                txtboxTrainID.Clear();
                comboBoxTrainDepartureStation.SelectedIndex = -1;
                comboBoxTrainArrivalStation.SelectedIndex = -1;
                comboBoxTrainDepartureTime.SelectedIndex = -1;
                comboBoxCabin.SelectedIndex = -1;
                comboBoxTrainFirstClass.SelectedIndex = -1;
                datepickerTrainDepartureDate.Text = "";
                comboBoxTrainType.SelectedIndex = -1;
            }
            else
            {
                MessageBox.Show("Train not added");
            }
        }

        private void btnAddPassenger_Click(object sender, RoutedEventArgs e)
        {
            passenger p = new passenger();
            seatNo = Convert.ToInt32(comboBoxPassengerSeat.Text);
            p.name = textboxPassengerName.Text;
            p.ID = comboBoxPassengerTrainID.Text;
            p.departure = comboBoxPassengerDepartureStation.Text;
            p.arrival = comboBoxPassengerArrivalStation.Text;
            p.coach = comboBoxPassengerCoach.Text;
            p.seat = seatNo;

            //try catch to make sure a seat is selected
            try
            {
                if (comboBoxPassengerSeat.SelectedIndex == -1)
                {
                    throw new System.ArgumentException("Must select a Seat", "Seat No");
                }
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
                errors = errors + 1;
            }
            //makes sure a train Id has been selected
            try
            {
                if (comboBoxPassengerTrainID.SelectedIndex == -1)
                {
                    throw new System.ArgumentException("Must select a Train ID", "Train ID");
                }
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
                errors = errors + 1;
            }
            //makes sure a name has been entered
            try
            {
                if (textboxPassengerName.Text == "")
                {
                    throw new System.ArgumentException("Invalid Name", "Name");
                }
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
                errors = errors + 1;
            }
            //makes sure a standard coach has a seat number between 1 and 60
            try
            {
                if (seatNo < 1 && seatNo > 60)
                {
                    throw new System.ArgumentException("Invalid Seat Number. Must be between 1 - 60", "Seat No");
                }
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
                errors = errors + 1;
            }
            // makes sure firstclass and cabin can only select a seat from 1- 20
            try
            {
                if (seatNo < 1 && seatNo > 20 && comboBoxPassengerCoach.Text == "FirstClass")
                {
                    throw new System.ArgumentException("Invalid Seat Number. Must be betweeen 1 - 20", "Seat No");
                }
                if (seatNo < 1 && seatNo > 20 && comboBoxPassengerCoach.Text == "Cabin")
                {
                    throw new System.ArgumentException("Invalid Seat Number. Must be betweeen 1 - 20", "Seat No");
                }
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
                errors = errors + 1;
            }
            // checks to ee if the seat is already booked
            try
            {
                if (passengers.ValidSeat(p.coach, p.seat, p.ID) == false)
                {
                    throw new System.ArgumentException("Seat Already booked", "Seat No");
                }
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
                errors = errors + 1;
            }
            //checks that the train goes between the selected stations
            try
            {
                if(trains.find(comboBoxPassengerTrainID.Text).stops != null)
                {
                    if (comboBoxPassengerDepartureStation.Text != trains.find(comboBoxPassengerTrainID.Text).departure || comboBoxPassengerArrivalStation.Text != trains.find(comboBoxPassengerTrainID.Text).destination)
                    {
                        if (comboBoxPassengerDepartureStation.Text == trains.find(comboBoxPassengerTrainID.Text).departure)
                        {
                            if (trains.find(comboBoxPassengerTrainID.Text).stops.Contains(comboBoxPassengerArrivalStation.Text) == false)
                            {
                                throw new ArgumentException("Train Doesn't stop there", "Train Departure, Train Arrival");
                            }

                        }
                        else if (comboBoxPassengerArrivalStation.Text == trains.find(comboBoxPassengerTrainID.Text).destination)
                        {
                            if (trains.find(comboBoxPassengerTrainID.Text).stops.Contains(comboBoxPassengerDepartureStation.Text) == false)
                            {
                                throw new ArgumentException("Train Doesn't stop there", "Train Arrival");
                            }
                        }
                        else if (trains.find(comboBoxPassengerTrainID.Text).stops.Contains(comboBoxPassengerDepartureStation.Text) == false && trains.find(comboBoxPassengerTrainID.Text).stops.Contains(comboBoxPassengerArrivalStation.Text) == false)
                        {
                            throw new ArgumentException("Train doesn't go between those two stations", "Train Departure, Train Arrival");
                        }
                    }
                }
                else if(trains.find(comboBoxPassengerTrainID.Text).stops == null)
                {
                    if(comboBoxPassengerDepartureStation.Text != trains.find(comboBoxPassengerTrainID.Text).departure && comboBoxPassengerArrivalStation.Text != trains.find(comboBoxPassengerTrainID.Text).destination)
                    {
                        throw new ArgumentException("Train doesn't have any stops");
                    }
                }
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
                errors = errors + 1;
            }
            
            //try catch to see if the passenger selected correct destination and departure if train doesn't have stops
            try
            {
                if (comboBoxPassengerTrainID.SelectedIndex >= 0)
                {
                    if (trains.find(comboBoxPassengerTrainID.Text).stops == null)
                    {
                        if (comboBoxPassengerDepartureStation.Text != trains.find(comboBoxPassengerTrainID.Text).departure && comboBoxPassengerArrivalStation.Text != trains.find(comboBoxPassengerTrainID.Text).destination)
                        {
                            throw new System.ArgumentException("The train doesn't go in that direction");
                        }
                    }
                }
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
                errors = errors + 1;
            }

            //try catch to check if destination and departure are in the right direction
            try
            {
                if (trains.find(comboBoxPassengerTrainID.Text).departure == "Edinburgh (Waverly)" && trains.find(comboBoxPassengerTrainID.Text).destination == "London (Kings Cross)")
                {
                    if (comboBoxPassengerArrivalStation.SelectedIndex < comboBoxPassengerDepartureStation.SelectedIndex)
                    {
                        throw new ArgumentException("Train doesn't go in that direction");
                    }
                }
                if (trains.find(comboBoxPassengerTrainID.Text).departure == "London (Kings Cross)" && trains.find(comboBoxPassengerTrainID.Text).destination == "Edinburgh (Waverly)")
                {
                    if (comboBoxPassengerArrivalStation.SelectedIndex > comboBoxPassengerDepartureStation.SelectedIndex)
                    {
                        throw new ArgumentException("Train doesn't go in that direction");
                    }
                }
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
                errors = errors + 1;
            }
            if (comboBoxPassengerDepartureStation.SelectedIndex >= 0 && comboBoxPassengerArrivalStation.SelectedIndex >= 0)
            {
                if (comboBoxPassengerDepartureStation.Text == "Edinburgh (Waverly)" && comboBoxPassengerArrivalStation.Text == "London (Kings Cross)" && trains.find(comboBoxPassengerTrainID.Text).type == "Stopper" || trains.find(comboBoxPassengerTrainID.Text).type == "Express" && p.coach == "A" || p.coach == "B" || p.coach == "C" || p.coach == "D" || p.coach == "E" || p.coach == "F" || p.coach == "G" || p.coach == "H" || comboBoxPassengerArrivalStation.Text == "Edinburgh (Waverly)" && comboBoxPassengerDepartureStation.Text == "London (Kings Cross)" && trains.find(comboBoxPassengerTrainID.Text).type == "Stopper" || trains.find(comboBoxPassengerTrainID.Text).type == "Express" && p.coach == "A" || p.coach == "B" || p.coach == "C" || p.coach == "D" || p.coach == "E" || p.coach == "F" || p.coach == "G" || p.coach == "H")
                {
                    ticketCost = 50;
                }
                else if (comboBoxPassengerDepartureStation.Text == "Edinburgh (Waverly)" && comboBoxPassengerArrivalStation.Text == "London (Kings Cross)" && trains.find(comboBoxPassengerTrainID.Text).type == "Stopper" || trains.find(comboBoxPassengerTrainID.Text).type == "Express" && trains.find(comboBoxPassengerTrainID.Text).firstClass == true && p.coach == "FirstClass" || comboBoxPassengerDepartureStation.Text == "London (Kings Cross)" && comboBoxPassengerArrivalStation.Text == "Edinburgh (Waverly)" || comboBoxPassengerArrivalStation.Text == "London (Kings Cross)" && trains.find(comboBoxPassengerTrainID.Text).type == "Stopper" || trains.find(comboBoxPassengerTrainID.Text).type == "Express" && trains.find(comboBoxPassengerTrainID.Text).firstClass == true && p.coach == "FirstClass")
                {
                    ticketCost = 60;
                }
                else if (comboBoxPassengerDepartureStation.Text == "Edinburgh (Waverly)" && comboBoxTrainArrivalStation.Text == "London (Kings Cross)" && trains.find(comboBoxPassengerTrainID.Text).type == "Sleeper" && p.coach == "A" || p.coach == "B" || p.coach == "C" || p.coach == "D" || p.coach == "E" || p.coach == "F" || p.coach == "G" || p.coach == "H" ||  comboBoxPassengerArrivalStation.Text == "Edinburgh (Waverly)" && comboBoxPassengerDepartureStation.Text == "London (Kings Cross)" && trains.find(comboBoxPassengerTrainID.Text).type == "Sleeper" && p.coach == "A" || p.coach == "B" || p.coach == "C" || p.coach == "D" || p.coach == "E" || p.coach == "F" || p.coach == "G" || p.coach == "H")
                {
                    ticketCost = 60;
                }
                else if (comboBoxPassengerDepartureStation.Text == "Edinburgh (Waverly)" && comboBoxPassengerArrivalStation.Text == "London (Kings Cross)" && trains.find(comboBoxPassengerTrainID.Text).firstClass == true && p.coach == "FirstClass" || comboBoxPassengerDepartureStation.Text == "London (Kings Cross)" && comboBoxPassengerArrivalStation.Text == "Edinburgh (Waverly)" && trains.find(comboBoxPassengerTrainID.Text).type == "Sleeper" && trains.find(comboBoxPassengerTrainID.Text).firstClass == true && p.coach == "FirstClass")
                {
                    ticketCost = 70;
                }
                else if (comboBoxPassengerDepartureStation.Text == "Edinburgh (Waverly)" && comboBoxPassengerArrivalStation.Text == "London (Kings Cross)" && trains.find(comboBoxPassengerTrainID.Text).type == "Sleeper" && trains.find(comboBoxPassengerTrainID.Text).sleeper == true && p.coach == "Cabin" || comboBoxPassengerDepartureStation.Text == "London (Kings Cross)" && comboBoxPassengerArrivalStation.Text == "Edinburgh (Waverly)" || comboBoxPassengerArrivalStation.Text == "London (Kings Cross)" && trains.find(comboBoxPassengerTrainID.Text).type == "Sleeper" && trains.find(comboBoxPassengerTrainID.Text).sleeper == true && p.coach == "Cabin")
                {
                    ticketCost = 80;
                }
                else if (comboBoxPassengerDepartureStation.Text == "Edinburgh (Waverly)" && comboBoxPassengerArrivalStation.Text == "Newcastle" || comboBoxPassengerDepartureStation.Text == "Edinburgh (Waverly)" && comboBoxPassengerArrivalStation.Text == "Darlington" || comboBoxPassengerDepartureStation.Text == "Edinburgh (Waverly)" &&  comboBoxPassengerArrivalStation.Text == "York" || comboBoxPassengerDepartureStation.Text == "Edinburgh (Waverly)" &&  comboBoxPassengerArrivalStation.Text == "Peterborough" && trains.find(comboBoxPassengerTrainID.Text).type == "Stopper" && p.coach == "A" || p.coach == "B" || p.coach == "C" || p.coach == "D" || p.coach == "E" || p.coach == "F" || p.coach == "G" || p.coach == "H" || comboBoxPassengerDepartureStation.Text == "London (Kings Cross)" && comboBoxPassengerArrivalStation.Text == "Newcastle" || comboBoxPassengerDepartureStation.Text == "London (Kings Cross)" && comboBoxPassengerArrivalStation.Text == "Darlington" || comboBoxPassengerDepartureStation.Text == "London (Kings Cross)" && comboBoxPassengerArrivalStation.Text == "York" || comboBoxPassengerDepartureStation.Text == "London (Kings Cross)" && comboBoxPassengerArrivalStation.Text == "Peterborough" && trains.find(comboBoxPassengerTrainID.Text).type == "Stopper" && p.coach == "A" || p.coach == "B" || p.coach == "C" || p.coach == "D" || p.coach == "E" || p.coach == "F" || p.coach == "G" || p.coach == "H")
                {
                    ticketCost = 25;
                }
                else if(comboBoxPassengerArrivalStation.Text == "Edinburgh (Waverly)" && comboBoxPassengerDepartureStation.Text == "Newcastle" || comboBoxPassengerArrivalStation.Text == "Edinburgh (Waverly)" && comboBoxPassengerDepartureStation.Text == "Darlington" || comboBoxPassengerArrivalStation.Text == "Edinburgh (Waverly)" && comboBoxPassengerDepartureStation.Text == "York" || comboBoxPassengerArrivalStation.Text == "Edinburgh (Waverly)" && comboBoxPassengerDepartureStation.Text == "Peterborough" && trains.find(comboBoxPassengerTrainID.Text).type == "Stopper" && p.coach == "A" || p.coach == "B" || p.coach == "C" || p.coach == "D" || p.coach == "E" || p.coach == "F" || p.coach == "G" || p.coach == "H" || comboBoxPassengerArrivalStation.Text == "London (Kings Cross)" && comboBoxPassengerDepartureStation.Text == "Newcastle" || comboBoxPassengerDepartureStation.Text == "London (Kings Cross)" && comboBoxPassengerDepartureStation.Text == "Darlington" || comboBoxPassengerArrivalStation.Text == "London (Kings Cross)" && comboBoxPassengerDepartureStation.Text == "York" || comboBoxPassengerArrivalStation.Text == "London (Kings Cross)" && comboBoxPassengerDepartureStation.Text == "Peterborough" && trains.find(comboBoxPassengerTrainID.Text).type == "Stopper" && p.coach == "A" || p.coach == "B" || p.coach == "C" || p.coach == "D" || p.coach == "E" || p.coach == "F" || p.coach == "G" || p.coach == "H")
                {
                    ticketCost = 25;
                }
                else if (comboBoxPassengerDepartureStation.Text != "Edinburgh (Waverly)" && comboBoxPassengerArrivalStation.Text != "London (Kings Cross)" && trains.find(comboBoxPassengerTrainID.Text).type == "Stopper" && p.coach == "A" || p.coach == "B" || p.coach == "C" || p.coach == "D" || p.coach == "E" || p.coach == "F" || p.coach == "G" || p.coach == "H" || comboBoxPassengerDepartureStation.Text != "London (Kings Cross)" && comboBoxPassengerArrivalStation.Text != "Edinburgh (Waverly)" && trains.find(comboBoxPassengerTrainID.Text).type == "Stopper" && p.coach == "A" || p.coach == "B" || p.coach == "C" || p.coach == "D" || p.coach == "E" || p.coach == "F" || p.coach == "G" || p.coach == "H")
                {
                    ticketCost = 25;
                }
                else if (comboBoxPassengerDepartureStation.Text == "Edinburgh (Waverly)" && comboBoxPassengerArrivalStation.Text == "Newcastle" || comboBoxPassengerArrivalStation.Text == "Darlington" || comboBoxPassengerArrivalStation.Text == "York" || comboBoxPassengerArrivalStation.Text == "Peterborough" && trains.find(comboBoxPassengerTrainID.Text).type == "Stopper" && p.coach == "FirstClass" || comboBoxPassengerDepartureStation.Text == "London (Kings Cross)" && comboBoxPassengerArrivalStation.Text == "Newcastle" || comboBoxPassengerArrivalStation.Text == "Darlington" || trains.find(comboBoxPassengerTrainID.Text).destination == "York" || comboBoxPassengerArrivalStation.Text == "Peterborough" && trains.find(comboBoxPassengerTrainID.Text).type == "Stopper" && p.coach == "FirstClass")
                {
                    ticketCost = 35;
                }
                else if (comboBoxPassengerDepartureStation.Text == "Edinburgh (Waverly)"  && comboBoxPassengerArrivalStation.Text == "Newcastle" || comboBoxPassengerArrivalStation.Text == "Darlington" || comboBoxPassengerArrivalStation.Text == "York" || comboBoxPassengerArrivalStation.Text == "Peterborough" && trains.find(comboBoxPassengerTrainID.Text).type == "Sleeper" && p.coach == "A" || p.coach == "B" || p.coach == "C" || p.coach == "D" || p.coach == "E" || p.coach == "F" || p.coach == "G" || p.coach == "H" || comboBoxPassengerDepartureStation.Text == "London (Kings Cross)" && comboBoxPassengerArrivalStation.Text == "Newcastle" || comboBoxPassengerArrivalStation.Text == "Darlington" || comboBoxPassengerArrivalStation.Text == "York" || comboBoxPassengerArrivalStation.Text == "Peterborough" && trains.find(comboBoxPassengerTrainID.Text).type == "Sleeper" && p.coach == "A" || p.coach == "B" || p.coach == "C" || p.coach == "D" || p.coach == "E" || p.coach == "F" || p.coach == "G" || p.coach == "H")
                {
                    ticketCost = 35;
                }
                else if (comboBoxPassengerDepartureStation.Text == "Edinburgh (Waverly)" && comboBoxPassengerArrivalStation.Text == "Newcastle" || comboBoxPassengerArrivalStation.Text == "Darlington" || comboBoxPassengerArrivalStation.Text == "York" || comboBoxPassengerArrivalStation.Text == "Peterborough" && trains.find(comboBoxPassengerTrainID.Text).type == "Sleeper" && p.coach == "FirstClass" || comboBoxPassengerDepartureStation.Text == "London (Kings Cross)" && comboBoxPassengerArrivalStation.Text == "Newcastle" || comboBoxPassengerArrivalStation.Text == "Darlington" || comboBoxPassengerArrivalStation.Text == "York" || comboBoxPassengerArrivalStation.Text == "Peterborough" && trains.find(comboBoxPassengerTrainID.Text).type == "Sleeper" && p.coach == "FirstClass")
                {
                    ticketCost = 45;
                }
                else if (comboBoxPassengerDepartureStation.Text == "Edinburgh (Waverly)" && comboBoxPassengerArrivalStation.Text == "Newcastle" || comboBoxPassengerArrivalStation.Text == "Darlington" || comboBoxPassengerArrivalStation.Text == "York" || comboBoxPassengerArrivalStation.Text == "Peterborough" && trains.find(comboBoxPassengerTrainID.Text).type == "Sleeper" && p.coach == "Cabin" || comboBoxPassengerDepartureStation.Text == "London (Kings Cross)" && comboBoxPassengerArrivalStation.Text == "Newcastle" || comboBoxPassengerArrivalStation.Text == "Darlington" || comboBoxPassengerArrivalStation.Text == "York" || comboBoxPassengerArrivalStation.Text == "Peterborough" && trains.find(comboBoxPassengerTrainID.Text).type == "Sleeper" && p.coach == "Cabin")
                {
                    ticketCost = 55;
                }
            }
            else
            {
                MessageBox.Show("Must select a train, departure, destination, cabin, seat no");
                errors = errors + 1;
            }

            if (p.coach == "FirstClass")
            {
                textBlockFirstClass.Text = "true";
            }
            else
            {
                textBlockFirstClass.Text = "false";
            }
            if (p.coach == "Cabin")
            {
                textBlockCabin.Text = "true";
            }
            else
            {
                textBlockCabin.Text = "false";
            }

            if (comboBoxPassengerCoach.Text == "FirstClass")
            {
                if (trains.find(comboBoxPassengerTrainID.Text).firstClass == false)
                {
                    MessageBox.Show("Cannot select first class", "Coach");
                    errors = errors + 1;
                }
            }

            if (comboBoxPassengerCoach.Text == "Cabin")
            {
                if (trains.find(comboBoxPassengerTrainID.Text).sleeper == false)
                {
                    MessageBox.Show("Cannot select cabin", "Coach");
                    errors = errors + 1;
                }
            }
         
            MessageBoxResult addBooking = MessageBox.Show("The ticket costs £" + ticketCost, "Add Booking", MessageBoxButton.YesNo);

            if(addBooking == MessageBoxResult.Yes && errors == 0)
            {
                passengers.add(p);
                listBoxTrainPassengers.Items.Add(p.ToString());
                textboxPassengerName.Clear();
                comboBoxPassengerTrainID.SelectedIndex = -1;
                comboBoxPassengerDepartureStation.SelectedIndex = -1;
                comboBoxPassengerArrivalStation.SelectedIndex = -1;
                comboBoxPassengerCoach.SelectedIndex = -1;
                comboBoxPassengerSeat.SelectedIndex = -1;
            }
            else
            {
                MessageBox.Show("Passenger wasn't Added");
                errors = 0;
            }
        }

        private void comboBoxTrainType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(comboBoxTrainType.SelectedIndex >= 1)
            {
                lblIntervalStations.Visibility = Visibility.Visible;
                checkBoxNewcastle.Visibility = Visibility.Visible;
                checkBoxDarlington.Visibility = Visibility.Visible;
                checkBoxYork.Visibility = Visibility.Visible;
                checkBoxPeterborough.Visibility = Visibility.Visible;
            }
            else
            {
                lblIntervalStations.Visibility = Visibility.Hidden;
                checkBoxNewcastle.Visibility = Visibility.Hidden;
                checkBoxDarlington.Visibility = Visibility.Hidden;
                checkBoxYork.Visibility = Visibility.Hidden;
                checkBoxPeterborough.Visibility = Visibility.Hidden;
            }
            if (comboBoxTrainType.SelectedIndex > -1)
            {
                btnAddTrain.IsEnabled = true;
            }
        }

        private void comboBoxDepartureStation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void comboBoxArrivalStation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void comboBoxTrainDepartureTime_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void comboBoxCabin_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void comboBoxTrainFirstClass_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void checkBoxNewcastle_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void checkBoxDarlington_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void checkBoxYork_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void checkBoxPeterborough_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void btnSearchDepartureDate_Click(object sender, RoutedEventArgs e)
        {
            listBoxTrainSearchResults.Items.Clear();
            if (datepickerSearchTrainDepartureDate.Text != "")
            {
                if (trains.IDs.Count > 0)
                {
                    foreach (string ID in trains.IDs)
                    {

                        if (trains.find(ID).day == datepickerSearchTrainDepartureDate.Text)
                        {
                            listBoxTrainSearchResults.Items.Add("Train ID: " + trains.find(ID).ID);
                        }
                        else if(trains.find(ID).day != datepickerSearchTrainDepartureDate.Text)
                        {
                            MessageBox.Show("No trains depart on that day");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("List is empty");
                }
            }
            else
            {
                MessageBox.Show("No date Selected", "Must Select a date");
            }
        }

        private void listBoxTrainSearchResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            trains.WriteToFile();
            passengers.WriteToFile();
        }

        private void comboBoxPassengerSeat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(comboBoxPassengerSeat.SelectedIndex != -1)
            {
                btnAddPassenger.IsEnabled = true;
            }
            else
            {
                btnAddPassenger.IsEnabled = false;
            }
        }

        private void btnTrainSatationSearch_Click(object sender, RoutedEventArgs e)
        {
            listBoxSearchTrain2Points.Items.Clear();

            if(trains.IDs.Count > 0)
            {
                if(comboBoxSearchArrivalStation.SelectedIndex >= 0 && comboBoxSearchDepartureStation.SelectedIndex >= 0)
                {
                    foreach (string ID in trains.IDs)
                    {
                        if (trains.find(ID).stops == null)
                        {
                            if (trains.find(ID).departure == comboBoxSearchDepartureStation.Text && comboBoxSearchArrivalStation.Text == trains.find(ID).destination)
                            {
                                listBoxSearchTrain2Points.Items.Add("Departure Day: " + trains.find(ID).day + " DepartreTime: " + trains.find(ID).time);
                            }
                        }
                        else
                        {
                            if (trains.find(ID).departure.Contains(comboBoxSearchDepartureStation.Text) || trains.find(ID).destination.Contains(comboBoxSearchArrivalStation.Text))
                            {
                                listBoxSearchTrain2Points.Items.Add("Departure Day: " + trains.find(ID).day + " DepartreTime: " + trains.find(ID).time);
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Must Select a departure Station and an arrival station");
                }
            }
        }
    }
}
