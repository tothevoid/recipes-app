﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Input;
using Microsoft.Win32;

namespace Catalog_of_recipes
{
    internal class AddRecipeView:ViewModelBase
    {
        #region Constructor
        public AddRecipeView()
        {
            Load_ingrs();
            Time = new List<string>() { "Праздничное", "Завтрак", "Обед", "Ужин" };
            Using_ingrs = new ObservableCollection<Ingredient>() {};
            Summary = "0: 0: 0: 0";
            Weight = "100";
            
        }
        #endregion

        #region Fields
        private string _name, _description, _message, _summary, _weight, _selectedTime;
        private int _searchselect;
        private Uri _image; 
        #endregion

        #region Properties
        public Uri Image{get { return _image; } set { Set(ref _image, value); }}
        public string Message { get { return _message; } set { Set(ref _message, value); } }
        public string SelectedTime { get { return _selectedTime; } set { Set(ref _selectedTime, value); } }
        public string Weight { get { return _weight; } set {Set(ref _weight,value); } }
        public string Summary {get { return _summary; } set {Set(ref _summary,value);} }
        public string Name { get { return _name; } set { Set(ref _name, value); } }
        public string Description { get { return _description; } set { Set(ref _description, value); } }
        public List<string> Time { get; set; }
        public ObservableCollection<Ingredient> Using_ingrs { get; set;}
        public int SearchSelect { get { return _searchselect; } set { Set(ref _searchselect,value);} }
       
    
        #endregion

        #region Methods

        private void Del(object parameter)
        {
            IList list = parameter as IList;
            List<Ingredient> Selected = list.Cast<Ingredient>().ToList();
            foreach (var i in Selected)
            {
                Using_ingrs.Remove(i);
            }
            CountSummary();
        }

        private void Add_recipe(object parameter)
        {
            bool isReady = FieldsCheck();
            if (isReady==false)
                return;
            if (Description == null)
                Description = "Отсутствует";
            List<double> props = Summary.Split(':').Select(x => double.Parse(x)).ToList();
            StringBuilder ingr = new StringBuilder();
            foreach (var x in Using_ingrs)
                ingr.Append(x.Name + "/" + x.Weight + "/");
            if (Temp.Count == Recipes.Count || Recipes.Count == 0)
                Recipes.Add(new Recipe { Name = Name, Time = SelectedTime, Description = Description, Pr = props[0], Fat = props[1], Ch = props[2], Cl = props[3], Ingredients = Convert.ToString(ingr) });
            Temp.Add(new Recipe { Name = Name, Time = SelectedTime, Description = Description, Pr = props[0], Fat = props[1], Ch = props[2], Cl = props[3], Ingredients = Convert.ToString(ingr) });
            Message = String.Format("{0} успешно добавлен ",Name);
            if (Image!=null)
            File.Copy(Image.LocalPath, Environment.CurrentDirectory + String.Format(@"\Images\{0}.png",Name));   
            Reset();
        }

        private bool FieldsCheck()
        {
            StringBuilder msg = new StringBuilder();
            if (Recipes.Any(i => Name == i.Name))
            {
                Message = "Такое название уже существует";
                return false;
            }
            if (string.IsNullOrEmpty(Name))
                    msg.Append("Имя, ");
            if (SelectedTime == null)
                    msg.Append("Время, ");
            if (Using_ingrs.Count == 0)
                    msg.Append("Ингредиенты ");
            if (msg.Length == 0)
                    return true;
                msg.Insert(0, "Необходимо заполнить: ");
                Message = Convert.ToString(msg).Substring(0,msg.Length-1);
                return false;
        }

        private void Add(object parameter)
        {
            
            var temp = Ingredients[SearchSelect];
            var dif = Math.Round(Convert.ToDouble(Weight) / temp.Weight,1);
            var temp2 = new Ingredient { Name = temp.Name, Pr = temp.Pr * dif, Ch = temp.Ch * dif, Fat = temp.Fat * dif, Cl = temp.Cl * dif, Weight = Convert.ToDouble(Weight)};
            int index = Check(temp);
            if (index == -1)
                Using_ingrs.Add(temp2);
            else
            {
                Using_ingrs[index].Ch += temp2.Ch;
                Using_ingrs[index].Fat += temp2.Fat;
                Using_ingrs[index].Cl += temp2.Cl;
                Using_ingrs[index].Pr += temp2.Pr;
                Using_ingrs[index].Weight += temp2.Weight;
                var update = Using_ingrs[index];
                Using_ingrs.RemoveAt(index);
                Using_ingrs.Add(update);
            }
            CountSummary();
        }

        
        private int Check(Ingredient temp)
        {
            int index = -1;
            for (int i = 0; i < Using_ingrs.Count; i++)
            {
                if (Using_ingrs[i].Name == temp.Name)
                    index = i;
            }
            return index;
        }

        private void Reset()
        {
            Name = null;
            Description = null;
            Image = null;
            SelectedTime= null;
            Summary = "0: 0: 0: 0";
            Using_ingrs.Clear();
        }

        private void Add_image(object parameter)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.FileName = "Image";
            dlg.Filter = "Image (.png)|*.png";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
                Image = new Uri(dlg.FileName);
        }

        private void CountSummary()
        {
            Item temp = new Item { Ch = 0, Cl = 0, Fat = 0, Pr = 0 };
            foreach (var i in Using_ingrs)
            {
                temp.Ch += i.Ch;
                temp.Fat += i.Fat;
                temp.Cl += i.Cl;
                temp.Pr += i.Pr;
            }
            Summary = string.Format("{0} : {1} : {2} : {3}", temp.Pr, temp.Ch, temp.Fat, temp.Cl);
        }
        #endregion

        #region Command
        public ICommand Add_Ingr
        {
            get { return new CommandBase(Add); }
        }

        public ICommand Add_rec
        {
            get { return new CommandBase(Add_recipe); }
        }

        public ICommand AddImage
        {
            get { return new CommandBase(Add_image); }
        }

        public ICommand Del_ingr
        {
            get { return new CommandBase(Del); }
        }
        #endregion

    }

}
