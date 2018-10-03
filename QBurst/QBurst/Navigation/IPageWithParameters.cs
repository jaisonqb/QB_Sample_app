using System;
using System.Collections.Generic;
using System.Text;

namespace QBurst.Navigation
{
    public interface IPageWithParameters
    {
        void InitializeWith(object parameter);
    }
}
