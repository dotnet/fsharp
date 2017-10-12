//<Expects status="Error" id="FS0039">The value or constructor 'xyz' is not defined.</Expects>
//<Expects>Maybe you want one of the following:</Expects>
//<Expects>\s+xy</Expects>
//<Expects status="notin">\s+_xyz</Expects>

let hober xy _xyz = xyz

exit 0
