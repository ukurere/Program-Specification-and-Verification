const form = document.getElementById('f');
const expr = document.getElementById('expr');
const msg  = document.getElementById('msg');
const viz  = document.getElementById('viz');

form.addEventListener('submit', async (e) => {
  e.preventDefault();
  msg.textContent = ''; viz.innerHTML = '';
  try {
    const r = await fetch('/api/parse', {
      method: 'POST',
      headers: {'Content-Type':'application/json'},
      body: JSON.stringify({expr: expr.value})
    });
    const data = await r.json();
    if(!data.ok) throw new Error(data.error || 'Parse error');
    drawTree(data.tree);
  } catch (err) { msg.textContent = err.message; }
});

function drawTree(treeData){
  const width=940, height=560, margin={top:30,right:20,bottom:20,left:20};
  const svg = d3.select('#viz').append('svg')
    .attr('viewBox',`0 0 ${width} ${height}`);
  const g = svg.append('g').attr('transform',`translate(${margin.left},${margin.top})`);
  const root = d3.hierarchy(treeData);
  d3.tree().size([width-margin.left-margin.right, height-margin.top-margin.bottom])(root);

  g.selectAll('.link').data(root.links()).enter().append('path')
    .attr('class','link')
    .attr('d', d3.linkVertical().x(d=>d.x).y(d=>d.y));

  const node = g.selectAll('.node').data(root.descendants()).enter().append('g')
    .attr('class','node').attr('transform',d=>`translate(${d.x},${d.y})`);
  node.append('circle').attr('r',16);
  node.append('text').attr('dy',4).attr('text-anchor','middle').text(d=>d.data.name);
}
