using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Coffee0417.LINE
{
    public class LINElogin
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //prop
        public int Id { get; set; }

        [Display(Name = "LINEid")]
        public string UserId { get; set; }

        [Display(Name = "displayName")]
        public string Name { get; set; }

        [Display(Name = "pictureUrl")]
        public string ImgUrl { get; set; }

        [Required]
        [Display(Name = "LINEemail")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Display(Name = "LINEaccess_token")]
        public string access_token { get; set; }

        [Display(Name = "LINEid_token")]
        public string id_token { get; set; }
    }
}