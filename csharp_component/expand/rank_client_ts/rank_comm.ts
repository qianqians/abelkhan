import * as client_handle from "./client_handle";
/*this enum code is codegen by abelkhan codegen for ts*/

/*this struct code is codegen by abelkhan codegen for typescript*/
export class rank_item
{
    public guid : number = 0;
    public score : number = 0;
    public rank : number = 0;
    public item : Uint8Array = Uint8Array.from([]);

}

export function rank_item_to_protcol(_struct:rank_item){
    return _struct;
}

export function protcol_to_rank_item(_protocol:any){
    if (_protocol == null) {
        return null;
    }

    let _struct = new rank_item();
    for (const [key, val] of Object.entries(_protocol)) {
        if (key === "guid"){
            _struct.guid = val as number;
        }
        else if (key === "score"){
            _struct.score = val as number;
        }
        else if (key === "rank"){
            _struct.rank = val as number;
        }
        else if (key === "item"){
            _struct.item = val as Uint8Array;
        }
    }
    return _struct;
}

/*this caller code is codegen by abelkhan codegen for typescript*/
