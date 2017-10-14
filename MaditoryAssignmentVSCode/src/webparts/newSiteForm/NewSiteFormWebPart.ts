import { Version } from '@microsoft/sp-core-library';
import {
  BaseClientSideWebPart,
  IPropertyPaneConfiguration,
  PropertyPaneTextField
} from '@microsoft/sp-webpart-base';
import { escape } from '@microsoft/sp-lodash-subset';

import styles from './NewSiteForm.module.scss';
import * as strings from 'newSiteFormStrings';
import { INewSiteFormWebPartProps } from './INewSiteFormWebPartProps';
import pnp from 'sp-pnp-js';

export default class NewSiteFormWebPart extends BaseClientSideWebPart<INewSiteFormWebPartProps> {
  
  protected onInit(): Promise<void> {  
    return super.onInit().then(_ => {
      pnp.setup({
        spfxContext: this.context
      }) 
    })
 }
  protected GetPreviewForm():Element{
        var divElement = document.createElement('div');        
        divElement.innerHTML=`                     
                <div>Title: <input type='text' id ='txtTitle'></input></div>
                <div>URL: <input type='url' id ='urlUrl'></input></div>
                <div>Owner: <input type='email' id ='emailOwner'></input></div>            
                <input type='button' id ='orderButton' class='orderBtn' value ='Order'></input>
                <input type='button' id ='cancelButton' class='cancelBtn' value='Cancel'></input>                  
                `;
        return divElement;
    }

    protected OrderNewSite():void{  
      // adding title, url and owner to the list
        var title = document.getElementById("txtTitle") as HTMLInputElement;
        var url = document.getElementById("urlUrl") as HTMLInputElement;
        var owner = document.getElementById("emailOwner") as HTMLInputElement;                
        
        pnp.sp.web.lists.getByTitle("NewSitesList").items.add(
            {
                Title: title.value,
                siteURL: url.value, 
                Owner: owner.value,                                            
            }
        ).then(() => {
            this.render();
            alert('New Site has been ordered');
        })   
    } 

    protected CancelNewSite():void{  
      
        var title = document.getElementById("txtTitle") as HTMLInputElement;
        var url = document.getElementById("urlUrl") as HTMLInputElement;
        var owner = document.getElementById("emailOwner") as HTMLInputElement;                
        
        title.value = null;
        url.value=null;
        owner.value=null;

        this.render();
        alert('Your input is cancelled. please reorder');
    } 

  public render(): void {
    this.domElement.innerHTML='';
    var previewFormElement = this.GetPreviewForm();
    this.domElement.appendChild(previewFormElement);
                                 
     // Order new site
     var orderElement = this.domElement.getElementsByClassName('orderBtn')[0];    
     var me = this; 
     orderElement.addEventListener('click', function(){       
        me.OrderNewSite();
               });  
    // Cancel new site
    var cancelElement = this.domElement.getElementsByClassName('cancelBtn')[0];     
     cancelElement.addEventListener('click', function(){      
       me.CancelNewSite();
               });  
  }



  protected get dataVersion(): Version {
    return Version.parse('1.0');
  }


}
