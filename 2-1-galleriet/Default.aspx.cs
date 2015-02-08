using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace _2_1_galleriet
{
    public partial class Default : System.Web.UI.Page
    {
        private void renderErrorMessage(string msg)
        {
            CustomValidator customValidator = new CustomValidator();
            customValidator.IsValid = false;
            customValidator.ErrorMessage = msg;
            this.Page.Validators.Add(customValidator);
        }

        private void renderImage(string imageSrc)
        {
            ImageContainer.Controls.Add(new HtmlImage() {
                Src=imageSrc
            });

            ImageContainer.Visible = true;
        }

        private void renderThumbnails()
        {
            IEnumerable<string> imageList;
            Gallery galleryObj = new Gallery();

            // Get previously uploaded images
            imageList = galleryObj.GetImageNames(true);

            ThumbnailRepeater.DataSource = imageList;
            ThumbnailRepeater.DataBind();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                string image;
                Gallery gallery = new Gallery();

                // Render thumbnails
                renderThumbnails();

                // Get image name from get request
                image = Request.QueryString["img"];

                // Render image only if it exists
                if (image != null && gallery.ImageExists(image))
                {
                    renderImage(String.Format("{0}/{1}", Gallery.IMAGE_PATH, image));
                }
            }
        }

        protected void UploadButton_Click(object sender, EventArgs e)
        {
            if(IsValid)
            {
                Gallery galleryObj;
                string fileName;

                // If no file was uploaded
                if(!FileUpload.HasFile)
                {
                    renderErrorMessage("Var god välj en fil som ska laddas upp.");
                    return;
                }

                // Create gallery object
                galleryObj = new Gallery();
                
                // Try to save image
                try
                {
                    fileName = galleryObj.SaveImage(
                                        FileUpload.PostedFile.InputStream,
                                        Path.GetFileName(FileUpload.PostedFile.FileName)
                                    );

                    InfoPanel.Visible = true;
                    InfoPanel.CssClass = "success-message";
                    InfoPanelLiteral.Text = String.Format("Bilden {0} har laddats upp.", fileName);


                    Response.Redirect(String.Format("Default.aspx?img={0}", fileName));

                }
                catch (Exception error)
                {
                    renderErrorMessage(error.Message);
                }
            }

            renderThumbnails();
            
        }
    }
}