using System;
using System.Collections.Generic;
using System.Text;

namespace MediaPlayer.Data.Factory.Abstraction;

public partial interface IRouteParameters
{
    #region Properties

    /// <summary>
    /// 
    /// </summary>
    List<Movie> Movies { get; set; }

    /// <summary>
    /// 
    /// </summary>
    Guid Token { get; set; }

    #endregion
}
