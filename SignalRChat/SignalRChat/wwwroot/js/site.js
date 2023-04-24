

//document.addEventListener("DOMContentLoaded", () => {

//  let resCell = tableElement.querySelectorAll("div.field");
//    for (let elem of resCell) {
//        Resize(elem);
//    }

//  alert("Hello World!");
//});

//function Resize(element)
//{
//    //alert("In");
//    let parent = element.parentNode;
//    let width = parent.clientWidth;
//    let height = parent.clientHeight;
//    let col = parent.colSpan;
//    let row = parent.rowSpan;
//    element.style.width = width / col + "px";
//    element.style.height = height / row +"px";
//    if (col>1)
//        alert(parent.getAttribute("name") + "element.offsetWidth: " + element.clientWidth + "///" + width + " element.offsetHeight: " + element.clientHeight +"///" + height);
//    if (row>1)
//        alert(parent.getAttribute("name") + "element.offsetWidth: " + element.clientWidth + "///" + width + " element.offsetHeight: " + element.clientHeight + "///" + height);
//}

//реализуем drag&drop
const tableElement = document.querySelector(`.battleship-table-1`);
//const tdActiveElements = tableElement.querySelectorAll(`.start_td_act`);

//// Перебираем все элементы списка и присваиваем нужное значение
////for (const td of tdActiveElements) {
////    td.draggable = true;
////}

//tableElement.addEventListener(`dragstart`, (evt) => {
//    var name = evt.target.getAttribute("name");
//    let dragCell = tableElement.querySelectorAll("div.d-td.start_td_act[name='" + name + "']");
//    for (let elem of dragCell) {
//        elem.classList.add(`selected`);
//    }
//    let parent = evt.target.parentNode;
//    var html = '';

//    // собираем HTML выделенных элементов.
//    for (let elem of dragCell) {
//        html += elem.outerHTML;
//    }
//    //var tempDiv = document.createElement("div");
//    //tempDiv.style.width = "100px";
//    //tempDiv.style.height = "100px";
//    //tempDiv.style.background = "green";

//    //tempDiv.innerHTML = html;
//    // parent.insertBefore(tempDiv, evt.target);
//    //alert(tempDiv);

//    // Устанавливаем собранный HTML в качестве данных для перетаскивания.
//    // Это никак не влияет на визуальную часть.
//    e.dataTransfer.effectAllowed = 'move';
//    e.dataTransfer.setData('text/html', html);


//   // evt.target.classList.add(`selected`);
//});



//tableElement.addEventListener(`dragend`, (evt) => {
//    var name = evt.target.getAttribute("name");
//    let dragCell = tableElement.querySelectorAll("div.d-td.start_td_act[name='" + name + "']");
//    for (let elem of dragCell) {
//        elem.classList.remove(`selected`);
//    }
//});

//tableElement.addEventListener(`drop`, (evt) => {
//    var name = evt.target.getAttribute("name");
//    let dragCell = tableElement.querySelectorAll("div.d-td.start_td_act[name='" + name + "']");
//    for (let elem of dragCell) {
//        elem.classList.remove(`selected`);
//    }
//});



//tableElement.addEventListener(`dragover`, (evt) => {
//    evt.preventDefault();
//    var name = evt.target.getAttribute("name");
//    let dragCell = tableElement.querySelectorAll("div.d-td.start_td_act[name='" + name + "']");

//    //for (let elem of dragCell) {
//    //    elem.preventDefault();
//    //}


//    const activeElement = tableElement.querySelectorAll(`.selected`);
//    const currentElement = evt.target;
//    const isMoveable = activeElement !== currentElement &&
//        currentElement.classList.contains(`start_td`);
//    if (!isMoveable) {
//        return;
//    }

//    var copy_from = activeElement.cloneNode(true);
//    activeElement.classList.remove(`selected`);
//    currentElement.replaceWith(copy_from);

//    var copy_to = currentElement.cloneNode(true);
//    activeElement.replaceWith(copy_to);


//});

tableElement.addEventListener(`dragover`, (evt) => {
    evt.preventDefault();

    const activeElement = tableElement.querySelectorAll(`.selected`);
    const currentElement = evt.target;
    const isMoveable = activeElement !== currentElement &&
        currentElement.classList.contains(`droppable`);
   
    if (!isMoveable) {
        return;
    }
   // currentElement.innerHTML = "88";

    //var copy_from = activeElement.cloneNode(true);
    //activeElement.classList.remove(`selected`);
    //currentElement.replaceWith(copy_from);

    //var copy_to = currentElement.cloneNode(true);
    //activeElement.replaceWith(copy_to);
});

document.addEventListener('DOMContentLoaded', (event) => {

    var dragSrcEl = null;

    function CheckCell(e, parent) {
        //получаем координаты проверяемой яйчейки
        let x = Number(e.getAttribute("pointx"));
        let y = Number(e.getAttribute("pointy"));
        let bool = true;
        //создаем массив координат окружающих ее яйчеек
        let points = [
            [(x - 1), (y - 1)],
            [(x - 1), y],
            [(x - 1), (y + 1)],
            [x, (y - 1)],
            [x, (y + 1)],
            [(x + 1), (y - 1)],
            [(x + 1), y],
            [(x + 1), (y + 1)]
        ];
        //удаляем родительские координаты чтобы их не проверять
        if (parent.rowSpan > 1 || parent.colSpan > 1) {
            // если это длинная яйчейка
            let cell = parent.querySelectorAll(`div`);
           // let j = 0;
            for (let child of cell) {
                let tempx = Number(child.getAttribute("pointx"));
                let tempy = Number(child.getAttribute("pointy"));
                for (var i = 0; i < points.length; i++)
                    if (points[i][0] == tempx && points[i][1] == tempy) {
                        points.splice(i, 1);                     
                    }               
            }
        }
        //если это одна яйчейка
        else {           
            let tempx = Number(parent.getAttribute("pointx"));
            let tempy = Number(parent.getAttribute("pointy"));
                for (var i = 0; i < points.length; i++)
                    if (points[i][0] == tempx && points[i][1] == tempy) {
                        points.splice(i, 1);
                    }            
        }
        //проверяем соседние точки
        for (var i = 0; i < points.length; i++) {
            if (points[i][0] > -1 && points[i][1] > -1) {
                let cls = "td[pointx=\"" + points[i][0] + "\"][pointy=\"" + points[i][1] + "\"]";
                let target = tableElement.querySelector(cls);
                if (target !== undefined && target !== null) {
                    if (target.classList.contains(`start_td_act`))
                    {
                        bool = false;
                    }                       
                }
                else {
                    cls = "div[pointx=\"" + points[i][0] + "\"][pointy=\"" + points[i][1] + "\"]";
                    target = tableElement.querySelector(cls);
                    if (target !== undefined && target !== null) {
                        if (target.classList.contains(`field`))
                        {
                            bool = false;
                        }
                            
                    }
                }
            }
        }  
        return bool;
    }

    function ChangeOver_(e, parent) {
        let x = Number(e.getAttribute("pointx"));
        let y = Number(e.getAttribute("pointy"));
        let arr = new Array();
        let points = [
                [(x - 1), (y - 1)],
                [(x - 1), y],
                [(x - 1), (y + 1)],
                [x, (y - 1)],
                [x, (y + 1)],
                [(x + 1), (y - 1)],
                [(x + 1), y],
                [(x + 1), (y + 1)]
            ];
       if (parent != null) {
            let cell = parent.querySelectorAll(`div`);
            for (let child of cell) {
                let tempx = Number(child.getAttribute("pointx"));
                let tempy = Number(child.getAttribute("pointy"));
                for (var i = 0; i < points.length; i++)
                    if (points[i][0] == tempx && points[i][1] == tempy) {
                        points.splice(i, 1); 
                    }
            } 
        }
        for (var i = 0; i < points.length; i++) {
            if (points[i][0] > -1 && points[i][1] > -1) {
                    let cls = "td[pointx=\"" + points[i][0] + "\"][pointy=\"" + points[i][1] + "\"]";
                        let target = tableElement.querySelector(cls);
                        if (target !== undefined && target !== null) {
                            arr.push(target);
                }
            }
        }
        return arr;
    }


    function handleDragStart(e) {
        this.style.opacity = '0.4';
        this.classList.add(`selected`);

        const dragCell = tableElement.querySelectorAll(`.droppable`);

        for (let elem of dragCell) {
            elem.classList.add('over');
        }

        let col = this.colSpan;
        let row = this.rowSpan;
        let arr = new Array();
        if ((row > 1) || (col > 1)) {
            let cell = this.querySelectorAll(`div`);
            for (let elem of cell) {
                arr.push(ChangeOver_(elem, this));               
            } 
        }
        else {
            arr.push(ChangeOver_(this, null));   
        }

        for (let elem of arr) {
            for (let point of elem) {
                let temp = CheckCell(point, this);
               if (temp) {
                   point.classList.add('droppable_temp');
                   point.classList.add('over');
                }
            }
        }

        dragSrcEl = this;

        e.dataTransfer.effectAllowed = 'move';
        e.dataTransfer.setData('text/html', this.innerHTML);
    }

    function handleDragOver(e) {
        if (e.preventDefault) {
            e.preventDefault();
        }
        e.dataTransfer.dropEffect = 'move';
        return false;
    }

    function handleDragEnter(e) {
       // this.classList.add('over');
       // this.innerHTML = "1";
    }

    function handleDragLeave(e) {
        //this.classList.remove('over');
       // this.innerHTML = "";
    }

    function handleDrop(e) {
        if (e.stopPropagation) {
            e.stopPropagation(); // stops the browser from redirecting.
        }

        if (dragSrcEl != this) {
            dragSrcEl.innerHTML = this.innerHTML;
            this.innerHTML = e.dataTransfer.getData('text/html');
        }

        return false;
    }

    function handleDragEnd(e) {
        this.style.opacity = '1';
        this.classList.remove(`selected`);
        const dragCell = tableElement.querySelectorAll(`.droppable`);

        for (let elem of dragCell) {
            elem.classList.remove('over');
        }
        const tempCell = tableElement.querySelectorAll(`.droppable_temp`);
        for (let elem of tempCell) {
            elem.classList.remove('over');
            elem.classList.remove('droppable_temp');
        }

      //  items.forEach(function (item) {
           
      //      this.innerHTML = "";
       // });
    }
    function handleMouseDown(e) {
        if ((e.button == 2) && ((this.rowSpan > 1) || (this.colSpan > 1)))
        {
            if (this.rowSpan > 1) {
                alert("Rowspan =" + this.rowSpan);
                this.colSpan = this.rowSpan;
                this.rowSpan = 1;
                RotateRow(this);
            }
            else alert("Colspan =" + this.colSpan);
        }
    }
    function RotateRow(e) {
        const tempCell = e.querySelectorAll(`div`);
        let head = tempCell[i];
        for (var i = 1; i < tempCell.length; i++) {
            //создаём яйчейки в замен повёрнутых
            //let td = document.createElement('td');
            //td.className = 'batelship_field_td start_td';
            //td.setAttribute("pointx", tempCell[i].getAttribute("pointx"));
            //td.setAttribute("pointy", tempCell[i].getAttribute("pointy"));
            //td.setAttribute("name", "empty");
            let newX = Number(tempCell[i].getAttribute("pointx"))+1;
            let cls = "td[pointx=\"" + newX + "\"][pointy=\"" + tempCell[i].getAttribute("pointy") + "\"]";
            alert(cls);
            let tdBefore = tableElement.querySelector(cls);
            if (tdBefore !== undefined && tdBefore !== null) {
                alert("INN");
                let parent = tdBefore.parentNode;
                var xI = Number(tempCell[i].getAttribute("pointx"));
                alert(xI);
                var x = parent.insertCell(xI);
                x.className = 'batelship_field_td start_td';
                x.setAttribute("name", "empty");
                x.setAttribute("pointx", tempCell[i].getAttribute("pointx"));
                x.setAttribute("pointy", tempCell[i].getAttribute("pointy"));               
              }
            //tableElement.insertBefore(td, tdBefore);
                 alert("111");
           // tempCell[i].classList.remove('over');            
        }
    }

    let items = document.querySelectorAll('.battleship-table-1 .start_td_act');
    items.forEach(function (item) {
    item.addEventListener('dragstart', handleDragStart);
  //  item.addEventListener('dragover', handleDragOver);
    item.addEventListener('dragenter', handleDragEnter);
    item.addEventListener('dragleave', handleDragLeave);
    item.addEventListener('dragend', handleDragEnd);
    item.addEventListener('drop', handleDrop);
   // item.addEventListener('mousedown', handleMouseDown);
    });



    let drop_items = document.querySelectorAll('.battleship-table-1 .start_td .droppable');
    drop_items.forEach(function (drop_item) {
  
      //  drop_item.addEventListener('dragstart', handleDragStart);
      //  drop_item.addEventListener('dragover', handleDragOver);
      //  drop_item.addEventListener('dragenter', handleDragEnter);
      //  drop_item.addEventListener('dragleave', handleDragLeave);
      //  drop_item.addEventListener('dragend', handleDragEnd);
      //  drop_item.addEventListener('drop', handleDrop);
    });
});