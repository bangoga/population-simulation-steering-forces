using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nodes : MonoBehaviour {

    public List<GameObject> neighbors;
    public List<string> topNumbers;
    public List<string> bottomNumbers;
    public List<string> rightNumbers;
    public List<string> leftNumbers;

    public int number;
    public void getNeighbor () {
        for (int i = 1; i <= 84; i=i+7){ string c = "Circle" + i; topNumbers.Add(c);}
        for (int i = 0; i <= 84; i = i + 7){ string c = "Circle" + i; bottomNumbers.Add(c);}
        for (int i = 2; i <= 6; i = i + 1) { string c = "Circle" + i; rightNumbers.Add(c); }
        for (int i = 79; i <= 83; i = i + 1) { string c = "Circle" + i; leftNumbers.Add(c); }

        if (topNumbers.Contains(this.transform.name) && (this.transform.name == "Circle1" || this.transform.name == "Circle8"))
        {
            number = Int32.Parse(""+this.transform.name[6]);
            int newNumber = number + 1;
            GameObject n1=GameObject.Find("Circle" + newNumber);
            if (n1 != null && !neighbors.Contains(n1)) { neighbors.Add(n1); }

            if (number < 78)
            {
                newNumber = number + 7;
                GameObject n2 = GameObject.Find("Circle" + newNumber);
                if (n2 != null && !neighbors.Contains(n2)) { neighbors.Add(n2); }
                
            }
           

            if (number > 1)
            {
                newNumber = number -7;
                GameObject n3 = GameObject.Find("Circle" + newNumber);
                if (n3 != null && !neighbors.Contains(n3)) { neighbors.Add(n3); }
                
            }


        }
        else if (topNumbers.Contains(this.transform.name)){
            number = Int32.Parse("" + this.transform.name[6]+""+ this.transform.name[7]);
            int newNumber = number + 1;
            GameObject n1 = GameObject.Find("Circle" + newNumber);

            if (n1 != null && !neighbors.Contains(n1)) { neighbors.Add(n1); }
            


            if (number < 78)
            {
                newNumber = number + 7;
                GameObject n2 = GameObject.Find("Circle" + newNumber);
                if (n2 != null && !neighbors.Contains(n2)) { neighbors.Add(n2); }
                
            }


            if (number > 1)
            {
                newNumber = number - 7;
                GameObject n3 = GameObject.Find("Circle" + newNumber);
                if (n3 != null && !neighbors.Contains(n3)) { neighbors.Add(n3); }
            }
        }

        else if (bottomNumbers.Contains(this.transform.name) && (this.transform.name == "Circle7"))
        {

            // One N above
            number = Int32.Parse("" + this.transform.name[6]);
            int newNumber = number - 1;
            GameObject n1 = GameObject.Find("Circle" + newNumber);
            if (n1 != null && !neighbors.Contains(n1)) { neighbors.Add(n1); }
            

            // One N Across
            newNumber = number + 7;
            GameObject n2 = GameObject.Find("Circle" + newNumber);
            if (n2 != null && !neighbors.Contains(n2)) { neighbors.Add(n2); }
        }
        else if (bottomNumbers.Contains(this.transform.name))
        {
            number = Int32.Parse("" + this.transform.name[6] + "" + this.transform.name[7]);
            int newNumber = number - 1;
            GameObject n1 = GameObject.Find("Circle" + newNumber);
            if (n1 != null && !neighbors.Contains(n1)) { neighbors.Add(n1); }
            


            if (number < 84)
            {
                newNumber = number + 7;
                GameObject n2 = GameObject.Find("Circle" + newNumber);
                if (n2 != null && !neighbors.Contains(n2)) { neighbors.Add(n2); }
                
            }


            if (number > 7)
            {
                newNumber = number - 7;
                GameObject n3 = GameObject.Find("Circle" + newNumber);
                if (n3 != null && !neighbors.Contains(n3)) { neighbors.Add(n3); }    
            }
        }

        else if (rightNumbers.Contains(this.transform.name))
        {
            number = Int32.Parse("" + this.transform.name[6]);

            int newNumber = number - 1;
            GameObject n1 = GameObject.Find("Circle" + newNumber);
            if (n1 != null && !neighbors.Contains(n1)) { neighbors.Add(n1); }
            

            newNumber = number + 1;
            GameObject n2 = GameObject.Find("Circle" + newNumber);
            if (n2 != null && !neighbors.Contains(n2)) { neighbors.Add(n2); }
            

            newNumber = number + 7;
            GameObject n3 = GameObject.Find("Circle" + newNumber);
            if (n3 != null && !neighbors.Contains(n3)) { neighbors.Add(n3); }
           
           
        }

        else if (leftNumbers.Contains(this.transform.name))
        {
            number = Int32.Parse("" + this.transform.name[6] + "" + this.transform.name[7]);

            int newNumber = number - 1;
            GameObject n1 = GameObject.Find("Circle" + newNumber);

            if (n1 != null && !neighbors.Contains(n1)) { neighbors.Add(n1); }
           

            newNumber = number + 1;
            GameObject n2 = GameObject.Find("Circle" + newNumber);
            if (n2 != null && !neighbors.Contains(n2)) { neighbors.Add(n2); }
            

            newNumber = number - 7;
            GameObject n3 = GameObject.Find("Circle" + newNumber);
            if (n3 != null && !neighbors.Contains(n3)) { neighbors.Add(n3); }
            
            
        }

        else
        {
            if (this.transform.name == "Circle9")
            {
                number = Int32.Parse("" + this.transform.name[6]);
            }
            else
            {
                number = Int32.Parse("" + this.transform.name[6] + "" + this.transform.name[7]);
            }

                int newNumber = number - 1;
                GameObject n1 = GameObject.Find("Circle" + newNumber);
                if (n1 != null && !neighbors.Contains(n1)) { neighbors.Add(n1); }
           

                newNumber = number + 1;
                GameObject n2 = GameObject.Find("Circle" + newNumber);
                if (n2 != null && !neighbors.Contains(n2)) { neighbors.Add(n2); }
           

                newNumber = number - 7;
                GameObject n3 = GameObject.Find("Circle" + newNumber);
                if (n3 != null && !neighbors.Contains(n3)) { neighbors.Add(n3); }
            


                newNumber = number + 7;
                GameObject n4 = GameObject.Find("Circle" + newNumber);
                if (n4 != null && !neighbors.Contains(n4)) { neighbors.Add(n4); }
            
        }

    }
}
