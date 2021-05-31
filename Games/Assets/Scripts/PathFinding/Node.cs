using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PathFinding
{
    public class Node<T>
    {
        public T Data { get { return mState; } }
        private T mState;

        public Node<T> Parent
        {
            get;
            set;
        }

        public int Cost
        {
            get;
        }

        public Node(T state, int cost = 0, Node<T> parent = null)
        {
            mState = state;
            //Cost = State.Cost() + depth;
            Cost = cost;
            Parent = parent;
        }

        // comparison operator based on the cost of the nodes. 
        // This can be used for sorting nodes based on the cost of 
        // the state that it holds.
        public static bool operator >(Node<T> n1, Node<T> n2)
        {
            return n1.Cost > n2.Cost;
        }

        public static bool operator <(Node<T> n1, Node<T> n2)
        {
            return n1.Cost < n2.Cost;
        }
    }
}
