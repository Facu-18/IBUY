using System;
using System.Collections.Generic;
using System.Text;

namespace Proyecto2026.BD.Datos
{
    public class EntityBase : IEntityBase
    {
        public int Id { get; set; }
    }

    public interface IEntityBase
    {
        public int Id { get; set; }
    }
}