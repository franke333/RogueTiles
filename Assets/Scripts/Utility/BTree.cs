using System;
using System.Collections.Generic;

public class BTree<T>
{
    public class Node
    {
        public T item;
        public Node left, right, parent;
        public bool IsList => left == null && right == null;

        public IEnumerator<Node> GetEnumerator()
        {
            if (left != null)
                foreach (var n in left)
                    yield return n;

            yield return this;

            if (right != null)
                foreach (var n in right)
                    yield return n;
        }

        public Node(T item)
        {
            this.item = item;
        }

        public Node SetChildLeft(T item)
        {
            left = new Node(item);
            left.parent = this;
            return left;
        }

        public Node SetChildRight(T item)
        {
            right = new Node(item);
            right.parent = this;
            return right;
        }
    }

    public Node root;

    public Node SetRoot(T item)
    {
        if (root != null)
            root.item = item;
        else
            root = new Node(item);
        return root;
    }

    public IEnumerator<Node> GetEnumerator() => root?.GetEnumerator();
}

