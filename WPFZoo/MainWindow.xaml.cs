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
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Collections;

namespace WPFZoo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SqlConnection sqlConnection;  //creates obj
        public MainWindow()
        {
            InitializeComponent();
            //SQLExpressDBConnectionString

            string ConnectionString = ConfigurationManager.ConnectionStrings["WPFZoo.Properties.Settings.SQLExpressDBConnectionString"].ConnectionString;
            sqlConnection = new SqlConnection(ConnectionString);
            ZooDisplay();
            AnimalSelect();
            textBox.Text = string.Empty;
            //AnimalDisplay();
        }

        //Zoo List diaplay
        private void ZooDisplay()
        {
            try
            {
                string query = "select * from Zoo";

                //sqladapter will run the query in the connection, i.e. to the database
                //it can be iamgined like an interface to make Tables usuable by C# obj
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, sqlConnection);
                using (sqlDataAdapter)
                {
                    DataTable dataTable = new DataTable();
                    sqlDataAdapter.Fill(dataTable);

                    listOfZoo.DisplayMemberPath = "Location";

                    //This sets the property name that will be used when retrieving the selected value from the list.
                    listOfZoo.SelectedValuePath = "Id";

                    //This assigns the data to the list. dataTable.DefaultView gives a bindable view of the data in a DataTable, often used in data binding scenarios.
                    listOfZoo.ItemsSource = dataTable.DefaultView;
                }

            }
            catch(Exception e) 
            {
                MessageBox.Show(e.ToString());
            }
           
        }

        private void ListOfZoo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //animals will be displayed when we click the Zoo location
            AnimalDisplay();
            SelectedZooTextBox();
        }
        private void AddAnimalList_selectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedAnimalTextBox();
        }

        //zoo animal display
        private void AnimalDisplay()
        {
            try
            {
                string queryAnimal = "SELECT * FROM Animal a INNER JOIN AnimalZoo az ON az.Animalid = a.Id where az.Zooid = @Zooid";

                SqlCommand sqlCommand = new SqlCommand(queryAnimal, sqlConnection);

                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                using (sqlDataAdapter)
                {

                    //assigning value to the variable @Zooid
                    sqlCommand.Parameters.AddWithValue("@Zooid", listOfZoo.SelectedValue);

                    DataTable animalTable = new DataTable();
                    sqlDataAdapter.Fill(animalTable);

                    AnimalList.DisplayMemberPath = "Name";

                    //This sets the property name that will be used when retrieving the selected value from the list.
                    AnimalList.SelectedValuePath = "Id";

                    //This assigns the data to the list. dataTable.DefaultView gives a bindable view of the data in a DataTable, often used in data binding scenarios.
                    AnimalList.ItemsSource = animalTable.DefaultView;
                }

            }

            catch( Exception e ) 
            {
                //MessageBox.Show(e.ToString());
            }
        }

        //add animal to zoo
        private void AnimalSelect()
        {
            try
            {
                string query = "select * from Animal";

                //sqladapter will run the query in the connection, i.e. to the database
                //it can be iamgined like an interface to make Tables usuable by C# obj
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, sqlConnection);
                using (sqlDataAdapter)
                {
                    DataTable AnimalSelectTable = new DataTable();
                    sqlDataAdapter.Fill(AnimalSelectTable);

                    AddAnimalList.DisplayMemberPath = "Name";

                    //This sets the property name that will be used when retrieving the selected value from the list.
                    AddAnimalList.SelectedValuePath = "Id";

                    //This assigns the data to the list. dataTable.DefaultView gives a bindable view of the data in a DataTable, often used in data binding scenarios.
                    AddAnimalList.ItemsSource = AnimalSelectTable.DefaultView;
                }

            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

        }


        private void DeleteZoo_Click(object sender, RoutedEventArgs e)
        {
            try {
                //MessageBox.Show("delete zoo");

                string query = "delete from Zoo where id = @Zooid";

                //alternative way to sqladapter

                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@Zooid", listOfZoo.SelectedValue);
                sqlCommand.ExecuteScalar();
            }
            catch(Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }

            finally
            {
                //even if there is an exception, the sql connection has to be closed, this block will always be executed
                sqlConnection.Close();

                ZooDisplay();

            }
        
            
        }


        private void AddZoo_click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("add zoo");

            try
            {
                string query = "insert into Zoo values (@Location)";

                //alternative way to sqladapter

                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@Location", textBox.Text);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }

            finally
            {
                //even if there is an exception, the sql connection has to be closed, this block will always be executed
                sqlConnection.Close();

                ZooDisplay();
                textBox.Text = string.Empty;

            }

        }

       

        private void AddAnimalZoo_click(object sender, RoutedEventArgs e)
        {

            try
            {
                string query = "insert into AnimalZoo values (@Zooid, @Animalid)"; //animalZoo has two parameters, we are inserting both values

                //alternative way to sqladapter

                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@Zooid", listOfZoo.SelectedValue); //listOfZoo.SelectedValue is the Zooid
                sqlCommand.Parameters.AddWithValue("@Animalid", AddAnimalList.SelectedValue); // AddAnimalList.SelectedValue is the Animalid
                sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }

            finally
            {
                //even if there is an exception, the sql connection has to be closed, this block will always be executed
                sqlConnection.Close();

                AnimalDisplay();

            }

        }

        private void RemoveAnimal_click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "delete from Animal where id = @Animalid"; //animalZoo has two parameters, we are inserting both values

                //alternative way to sqladapter

                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@Animalid", AnimalList.SelectedValue); // AddAnimalList.SelectedValue is the Animalid
                sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }

            finally
            {
                //even if there is an exception, the sql connection has to be closed, this block will always be executed
                sqlConnection.Close();

                AnimalDisplay();

            }

        }

       
        private void DeleteAnimal_click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "delete from Animal where id = @Animalid"; //animalZoo has two parameters, we are inserting both values

                //alternative way to sqladapter

                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@Animalid", AddAnimalList.SelectedValue); // AddAnimalList.SelectedValue is the Animalid
                sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }

            finally
            {
                //even if there is an exception, the sql connection has to be closed, this block will always be executed
                sqlConnection.Close();

                AnimalSelect();

            }

        }

        private void AddAnimal_click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "insert into Animal values (@Name)";

                //alternative way to sqladapter

                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@Name", textBox.Text);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }

            finally
            {
                //even if there is an exception, the sql connection has to be closed, this block will always be executed
                sqlConnection.Close();

                AnimalSelect();
                //textBox.Text = string.Empty;

            }

        }

        //this method is called to show the selected location in the zoo list in the textbox
        private void SelectedZooTextBox()
        {
            try
            {
                string query = "select Location from Zoo where Id = @ZooId";
                
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand);

                using (adapter)
                {
                    sqlCommand.Parameters.AddWithValue("@ZooId", listOfZoo.SelectedValue);
                    DataTable ZoodataTable = new DataTable();
                    adapter.Fill(ZoodataTable);
                    //textBox.Text = ZoodataTable.Rows[0]["Location"].ToString();
                    if (ZoodataTable.Rows.Count > 0)
                    {
                        textBox.Text = ZoodataTable.Rows[0]["Location"].ToString();
                    }
                    else
                    {
                        textBox.Text = "Location not found";
                    }

                }

            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        //this method is called to show the selected Name in the Animal list in the textbox
        private void SelectedAnimalTextBox()
        {
            try
            {
                string query = "select Name from Animal where id = @Animalid";

                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand);

                using (adapter)
                {
                    sqlCommand.Parameters.AddWithValue("@Animalid", AddAnimalList.SelectedValue);
                    DataTable ZoodataTable = new DataTable();
                    adapter.Fill(ZoodataTable);
                    //textBox.Text = ZoodataTable.Rows[0]["Location"].ToString();
                    if (ZoodataTable.Rows.Count > 0)
                    {
                        textBox.Text = ZoodataTable.Rows[0]["Name"].ToString();
                    }
                    else
                    {
                        textBox.Text = "Location not found";
                    }

                }

            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void UpdateZoo_click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "update Zoo Set Location = @Location where Id = @ZooId";

                //alternative way to sqladapter

                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@ZooId", listOfZoo.SelectedValue);
                sqlCommand.Parameters.AddWithValue("@Location", textBox.Text);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {

                //MessageBox.Show(ex.ToString());
            }

            finally
            {
                //even if there is an exception, the sql connection has to be closed, this block will always be executed
                sqlConnection.Close();

                ZooDisplay();
                //textBox.Text = string.Empty;

            }

        }

        private void UpdateAnimal_click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "update Animal Set Name = @Name where Id = @AnimalId";

                //alternative way to sqladapter

                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@Name", textBox.Text);
                sqlCommand.Parameters.AddWithValue("@AnimalId", AddAnimalList.SelectedValue);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }

            finally
            {
                //even if there is an exception, the sql connection has to be closed, this block will always be executed
                sqlConnection.Close();

                AnimalSelect();
                //textBox.Text = string.Empty;

            }
        }

        //when selecting animals from AddAnimalList box

    }
}
