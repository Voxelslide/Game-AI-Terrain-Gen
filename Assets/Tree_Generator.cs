﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tree_Generator : MonoBehaviour {
    
    private List<int> numberSegments = new List<int>();
    
    public float segBottomRadius = .55f;
	public float segTopRadius = .15f;
	
	public float segLength = 1.0f;
    
    private List<float> curBotRadius = new List<float>();
    private List<float> curTopRadius = new List<float>();
    
    private List<Vector3> gvertices = new List<Vector3>();
    private List<Vector3> gnormales = new List<Vector3>();
    private List<Vector2> guvs = new List<Vector2>();
    private List<int> gtriangles = new List<int>();
    
    private List<int> topVertices = new List<int>();
    
    private List<int> minVertex = new List<int>();
    private List<int> maxVertex = new List<int>();
    
    private List<int> minTriangle = new List<int>();
    private List<int> maxTriangle = new List<int>();
    
    private int currentSegmentId = 0;
    private int currentSegmentOffset = 0;
    
    private int currentBranchId = 0;
    
    private int verticesOffset = 0;
    private int trianglesOffset = 0;
    
    private List<float> angle = new List<float>();
    private float iniAngle = 15.0f;
    
    
    private List<Vector3> segmentPos = new List<Vector3>();
    private List<Quaternion> segmentRot = new List<Quaternion>();
    
    private List<Vector3> segmentLocRotVect = new List<Vector3>();
    
    private int nBranchesToAdd = 0;
    
    private int nbSides = 18;
    
    private List<int> inhSegId = new List<int>();
    private List<int> inhBranchId = new List<int>();
    
    private List<int> branchingOrder = new List<int>();
    
	//initialization
	void Start () {
		//MakeTree(0f, 0f, 0f);
	}
	
	//update
	void Update () {
	
	}

	public GameObject MakeTree() {
		SetupCone();
		return CreateMesh();
	}
	
	void SetupCone(){
	    
	    numberSegments.Add(30);
	
		iniAngle = 0.0f/segLength;
		
		angle.Add(0.0f);
		
		curBotRadius.Add(0f);
		curTopRadius.Add(0f);
		
		inhSegId.Add(currentSegmentId);
		inhBranchId.Add(0);
		
		branchingOrder.Add(0);
		
		Vector3 iniPos = new Vector3(0f, segLength, 0f);
		
		Vector3 rotVect = new Vector3(0f, 0f, 0f + 1);

		segmentPos.Add(new Vector3(0f, 0f, 0f));
		segmentLocRotVect.Add(new Vector3(0f, 0f+1, 0f));
		
	//	segmentRot.Add(Quaternion.AngleAxis( 0.5f*iniAngle, rotVect));
		segmentRot.Add(Quaternion.AngleAxis( iniAngle, rotVect));
		
		curBotRadius[currentBranchId] = segBottomRadius;
		curTopRadius[currentBranchId] = segBottomRadius-(segBottomRadius-segTopRadius)/numberSegments[currentBranchId];
		
		DrawCone();
		float branchPosibility = 1.0f;
		
		rotVect = Vector3.Cross
	    	(
	    		(segmentLocRotVect[currentBranchId]), 
	    		new Vector3(Random.Range(0f-1,0f+1f), Random.Range(0f-1,0f+1f), Random.Range(0f-1,0f+1f))
	    													
	    	).normalized;
	    
		for(int j = 1; j < numberSegments[currentBranchId]; j++){
		  /*  rotVect = Vector3.Cross
	    	(
	    		(segmentLocRotVect[currentBranchId]), 
	    		new Vector3(Random.Range(-1,1f),Random.Range(-1,1f),Random.Range(-1,1f))
	    													
	    	).normalized;*/
	    //    print(rotVect);
		    
		    angle[currentBranchId] = angle[currentBranchId] + iniAngle;
		    segmentRot[currentBranchId] = Quaternion.AngleAxis( angle[currentBranchId], rotVect);
		    
			segmentPos[currentBranchId] = segmentPos[currentBranchId]+(segmentRot[currentBranchId]*iniPos);
			currentSegmentOffset = 0;
			
			curTopRadius[currentBranchId] = segBottomRadius-(segBottomRadius-segTopRadius)*(j+1)/numberSegments[currentBranchId];
			DrawCone();
		
		
			for(int i=minVertex[currentSegmentId-1];i<maxVertex[currentSegmentId-1];i++){
				if(topVertices[i]==1){
					gvertices[i]=gvertices[i-currentSegmentOffset-1];
				}
			}
			
			branchPosibility = branchPosibility-0.1f;
			if((j>3)&&(Random.Range(0f,1f)>branchPosibility)){
				AddBranch();
				numberSegments.Add(numberSegments[currentBranchId]-j);
				branchPosibility = 1.0f;
				
			}
            
		}
		
		while(nBranchesToAdd>0){
			currentBranchId++;	

	    //    if(currentBranchId>0){
	    
	        Vector3 randVect = new Vector3(Random.Range(0f-1,0f+1f), Random.Range(0f-1,0f+1f), Random.Range(0f-1,0f+1f));
	        
			rotVect = Vector3.Cross
	    	(
	    		segmentLocRotVect[currentBranchId], 
	    		randVect											
	    	).normalized;
	    //	}	
	        angle[currentBranchId] = 0f;
			iniAngle = Random.Range(3f,7f)/segLength*((iniAngle+0.001f)/Mathf.Abs((iniAngle+0.001f)));
			
			if(branchingOrder[currentBranchId] == 3){
				print(segmentLocRotVect[currentBranchId]);
			}
		//	print(branchingOrder[currentBranchId]);
		//	print(segmentLocRotVect[currentBranchId]);
	
		//	segmentRot[currentBranchId] = Quaternion.AngleAxis( iniAngle, rotVect );
	        if(inhBranchId[currentBranchId]>0){
				segmentRot[currentBranchId] = (Quaternion.AngleAxis( 0.5f*iniAngle, rotVect));
			}
			else{
				//segmentRot[currentBranchId] = (Quaternion.AngleAxis( iniAngle, rotVect));
			}
			
			segBottomRadius = curTopRadius[currentBranchId];

		
			DrawCone();
			for(int i=minVertex[currentSegmentId-1];i<maxVertex[currentSegmentId-1];i++){
					if(topVertices[i]==1){
						gvertices[i]=gvertices[i-(minVertex[currentSegmentId-1]-minVertex[inhSegId[currentBranchId]])];
					}
			}
	//		print(inhSegId[currentBranchId]);
			
		/*	for(int i=minVertex[currentSegmentId-1];i<maxVertex[currentSegmentId-1];i++){
					if(topVertices[i]==1){
						gvertices[i]=gvertices[i-1];
					}
			}*/
		
		    
			
		    branchPosibility = 1.0f;
			for(int j=1;j<numberSegments[currentBranchId];j++){
		
		        angle[currentBranchId] = angle[currentBranchId]+iniAngle;
				segmentRot[currentBranchId] = Quaternion.AngleAxis( angle[currentBranchId], rotVect );
			
				segmentPos[currentBranchId] = segmentPos[currentBranchId]+(segmentRot[currentBranchId]*iniPos);
				currentSegmentOffset = 0;
			
				curTopRadius[currentBranchId] = segBottomRadius-(segBottomRadius-segTopRadius)*(j+1)/numberSegments[currentBranchId];
		
			    branchPosibility = branchPosibility-0.1f;
				if((j>2)&&(Random.Range(0f,1f)>branchPosibility)){
					AddBranch();
					numberSegments.Add(numberSegments[currentBranchId]-j);
					branchPosibility = 1.0f;
				}
			
				DrawCone();
		
				for(int i=minVertex[currentSegmentId-1];i<maxVertex[currentSegmentId-1];i++){
					if(topVertices[i]==1){
						gvertices[i]=gvertices[i-currentSegmentOffset-1];
					}
				}
			}
			nBranchesToAdd--;
		}
	}	
	
	void AddBranch(){
	    inhSegId.Add(currentSegmentId);
	    inhBranchId.Add(currentBranchId);
	    
		curBotRadius.Add(curBotRadius[currentBranchId]);
		curTopRadius.Add(curTopRadius[currentBranchId]);
		
		segmentPos.Add(segmentPos[currentBranchId]);
		segmentRot.Add(segmentRot[currentBranchId]);
		
		segmentLocRotVect.Add(segmentLocRotVect[currentBranchId]);
		
	//	angle.Add(angle[currentBranchId]);
		angle.Add(0f);
		nBranchesToAdd++;
		
		branchingOrder.Add(branchingOrder[currentBranchId]+1);
		
//		print(segmentPos[currentBranchId]);
	}
	void DrawCone(){

	int useBottomCap = 0;
	int useTopCap = 0; 
 
	float height = segLength;
	float bottomRadius = curBotRadius[currentBranchId];
	float topRadius = curTopRadius[currentBranchId];

	int nbVerticesCap = nbSides + 1;
	
	
//Vertices
	int NN = nbVerticesCap + nbVerticesCap + nbSides * 2 + 2;
	
	int[] vUsed = new int[NN];
	int[] isTopVertice = new int[NN];
	
	int numUsed = 0;
	
	for(int ii=0;ii<NN;ii++){
		vUsed[ii] = 0;
		isTopVertice[ii] = 0;
	}

	// bottom + top + sides
	Vector3[] vertices = new Vector3[nbVerticesCap + nbVerticesCap + nbSides * 2 + 2];
	int vert = 0;
	float _2pi = Mathf.PI * 2f;
 
	// Bottom cap
	if(useBottomCap == 1){
		vert = 0;
		vUsed[vert] = 1;
		vertices[vert++] = new Vector3(0f, 0f, 0f);
		numUsed++;
		while( vert <= nbSides )
		{
			float rad = (float)vert / nbSides * _2pi;
			vertices[vert] = new Vector3(Mathf.Cos(rad) * bottomRadius, 0f, Mathf.Sin(rad) * bottomRadius);
			vUsed[vert] = 1;
			
			numUsed++;
			vert++;
		}
	}
 
	// Top cap
	if(useTopCap == 1){
		vert = nbSides+1;
		vUsed[vert] = 1;
		vertices[vert++] = new Vector3(0f, height, 0f);
		numUsed++;
		while (vert <= nbSides * 2 + 1)
		{
			float rad = (float)(vert - nbSides - 1)  / nbSides * _2pi;
			vertices[vert] = new Vector3(Mathf.Cos(rad) * topRadius, height, Mathf.Sin(rad) * topRadius);
			vUsed[vert] = 1;
			numUsed++;
			vert++;
		}
	}

	vert = nbSides * 2 + 2; 
	// Sides
	int v = 0;
	while (vert <= vertices.Length - 4 )
	{
		float rad = (float)v / nbSides * _2pi;
		vertices[vert] = new Vector3(Mathf.Cos(rad) * topRadius, height, Mathf.Sin(rad) * topRadius);
		vertices[vert + 1] = new Vector3(Mathf.Cos(rad) * bottomRadius, 0, Mathf.Sin(rad) * bottomRadius);
		
		isTopVertice[vert + 1] = 1;
		
		vUsed[vert] = 1;
		vUsed[vert+1] = 1;
		
		vert+=2;
		numUsed+=2;
		v++;
	}
	vertices[vert] = vertices[ nbSides * 2 + 2 ];
	vertices[vert + 1] = vertices[nbSides * 2 + 3 ];
	
	isTopVertice[vert + 1] = 1;
	
	vUsed[vert] = 1;
	vUsed[vert+1] = 1;
	
    Vector3[] verticesA = new Vector3[numUsed+2];
    
    int jj = 0;
	for(int ii=0;ii<NN;ii++){
	    if(vUsed[ii] == 1){
			verticesA[jj] = vertices[ii];
			vertices[ii] = segmentRot[currentBranchId]*vertices[ii];
			gvertices.Add(vertices[ii]+segmentPos[currentBranchId]);
			
			jj++;
		}
	}
	segmentLocRotVect[currentBranchId] = (segmentRot[currentBranchId]*(new Vector3(0f, 1f, 0f) - new Vector3(0f, 0f, 0f))).normalized;
	//print(segmentRot[currentBranchId]*(new Vector3(0f, 1f, 0f) - new Vector3(0f, 0f, 0f)));
	jj = 0;
	for(int ii=0;ii<NN;ii++){
	    if(vUsed[ii] == 1){
			
			if(isTopVertice[ii]==1){
				topVertices.Add(1);
			}
			else{
				topVertices.Add(0);
			}
			
			jj++;
		}
	}
 
//Normales
	// bottom + top + sides
	Vector3[] normalesA = new Vector3[verticesA.Length];
	Vector3[] normales = new Vector3[vertices.Length];
	
    numUsed=0;
	vert = 0;
    
    for(int ii=0;ii<NN;ii++){
		vUsed[ii] = 0;
	}
 
	// Bottom cap
	if(useBottomCap == 1){
		vert = 0;
		while( vert  <= nbSides )
		{   
			normales[vert] = Vector3.down;
			vUsed[vert] = 1;
			numUsed++;
			vert++;
		}
	}
 
	// Top cap
	if(useTopCap == 1){
		vert = nbSides+1;
		while( vert <= nbSides * 2 + 1 )
		{
		    
			normales[vert] = Vector3.up;
			vUsed[vert] = 1;
			numUsed++;
			vert++;
		}
	}
  
	// Sides
	vert = nbSides * 2 + 2;
	v = 0;
	while (vert <= vertices.Length - 4 )
	{			
		float rad = (float)v / nbSides * _2pi;
		float cos = Mathf.Cos(rad);
		float sin = Mathf.Sin(rad);
 
		normales[vert] = new Vector3(cos, 0f, sin);
		normales[vert+1] = normales[vert];
		
		vUsed[vert] = 1;
		vUsed[vert+1] = 1;
 
        numUsed+=2;
		vert+=2;
		v++;
	}
	normales[vert] = normales[ nbSides * 2 + 2 ];
	normales[vert + 1] = normales[nbSides * 2 + 3 ];
	vUsed[vert] = 1;
	vUsed[vert+1] = 1;

    jj=0;
	for(int ii=0;ii<NN;ii++){
	    if(vUsed[ii] == 1){
			normalesA[jj] = normales[ii];
			gnormales.Add(normales[ii]);
			jj++;
		}
	}
 
//UVs
	Vector2[] uvsA = new Vector2[verticesA.Length];
	Vector2[] uvs = new Vector2[vertices.Length];
 
    int[] uUsed = new int[vertices.Length];
    
    for(int ii=0;ii<NN;ii++){
		uUsed[ii] = 0;
	}
 
	// Bottom cap
	numUsed=0;
	
	int u = 0;
	if(useBottomCap == 1){
		u = 0;
        
        uUsed[u] = 1;
		uvs[u++] = new Vector2(0.5f, 0.5f);
		numUsed++;
	
		while (u <= nbSides)
		{
			float rad = (float)u / nbSides * _2pi;
			uvs[u] = new Vector2(Mathf.Cos(rad) * .5f + .5f, Mathf.Sin(rad) * .5f + .5f);
			uUsed[u] = 1;
			numUsed++;
			u++;
		}
	} 
	// Top cap
	if(useTopCap == 1){
		u = nbSides+1;
	    
	    uUsed[u] = 1;
		uvs[u++] = new Vector2(0.5f, 0.5f);
		numUsed++;
		while (u <= nbSides * 2 + 1)
		{
			float rad = (float)u / nbSides * _2pi;
			uvs[u] = new Vector2(Mathf.Cos(rad) * .5f + .5f, Mathf.Sin(rad) * .5f + .5f);
			uUsed[u] = 1;
			numUsed++;
			u++;
		}
	}
	u = nbSides * 2 + 2;  
 
	// Sides
	int u_sides = 0;
	while (u <= uvs.Length - 4 )
	{
		float t = (float)u_sides / nbSides;
		uvs[u] = new Vector3(t, 1f);
		uvs[u + 1] = new Vector3(t, 0f);
		uUsed[u] = 1;
		uUsed[u+1] = 1;
		numUsed+=2;
		u += 2;
		u_sides++;
	}
	uvs[u] = new Vector2(1f, 1f);
	uvs[u + 1] = new Vector2(1f, 0f);
	uUsed[u] = 1;
	uUsed[u+1] = 1;
    
    
    jj=0;
	for(int ii=0;ii<NN;ii++){
	    if(uUsed[ii] == 1){
			uvsA[jj] = uvs[ii];
			guvs.Add(uvs[ii]);
			jj++;
		}
	}
 
//Triangles
	int nbTriangles = nbSides + nbSides + nbSides*2;
	int[] triangles = new int[nbTriangles * 3 + 3];

	int NT = nbTriangles * 3 + 3;

	int[] tUsed = new int[NT];
	
	for(int ii=0;ii<NT;ii++){
		tUsed[ii] = 0;
	}

    numUsed=0;
	// Bottom cap
	int tri = 0;
	int i = 0;
	
	int missTris = 0;
	
	if(useBottomCap == 1){
		while (tri < nbSides - 1)
		{
			if(useBottomCap == 1){
				triangles[ i ] = 0;
				triangles[ i+1 ] = tri - missTris + 1;
				triangles[ i+2 ] = tri - missTris + 2;
			
				tUsed[i] = 1;
				tUsed[i+1] = 1;
				tUsed[i+2] = 1;
			}
			numUsed+=3;
			tri++;
			i += 3;
		}
			triangles[i] = 0;
			triangles[i + 1] = tri - missTris + 1;
			triangles[i + 2] = 1;
		
			tUsed[i] = 1;
			tUsed[i+1] = 1;
			tUsed[i+2] = 1;
		
			numUsed+=3;
	
		tri++;
		i += 3;
	}
    
    if(useBottomCap == 1){
        if(useTopCap == 1){
    		missTris = 0;
    	}
    	if(useTopCap == 0){
    		missTris = 0;
    	}
    }
    else if(useBottomCap == 0){
        if(useTopCap == 1){
    		missTris = nbSides+1;
    	}
    	else if(useTopCap == 0){
    		missTris = nbSides+1;
    	}
    }
      
	tri = nbSides;
	i = 3*tri;
 
	// Top cap
	if(useTopCap == 1){
		while (tri < nbSides*2)
		{
			triangles[ i ] = tri - missTris + 2;
			triangles[i + 1] = tri - missTris + 1;
			triangles[i + 2] = nbVerticesCap - missTris;
			
			tUsed[i] = 1;
			tUsed[i+1] = 1;
			tUsed[i+2] = 1;
	        
	        numUsed+=3;
	        
			tri++;
			i += 3;	
		}

		triangles[i] = nbVerticesCap - missTris + 1;
		triangles[i + 1] = tri - missTris + 1;
		triangles[i + 2] = nbVerticesCap - missTris;	
		
		tUsed[i] = 1;
		tUsed[i+1] = 1;
		tUsed[i+2] = 1;
	    
	    numUsed+=3;
		tri++;
		i += 3;
		tri++;
	}
	
    if(useBottomCap == 1){
        if(useTopCap == 1){
    		missTris = 0;
    	}
    	if(useTopCap == 0){
    		missTris = nbSides+1;
    	}
    }
    else if(useBottomCap == 0){
        if(useTopCap == 1){
    		missTris = nbSides+1;
    	}
    	else if(useTopCap == 0){
    		missTris = nbSides*2+2;
    	}
    }
    
	tri = nbSides*2+2;
	i = 3*tri-3;
    
	// Sides
	while( tri <= nbTriangles )
	{
		triangles[ i ] = tri - missTris + 2;
		triangles[ i+1 ] = tri - missTris + 1;
		triangles[ i+2 ] = tri - missTris + 0;
		
		tUsed[i] = 1;
		tUsed[i+1] = 1;
		tUsed[i+2] = 1;
		
		numUsed+=3;
		tri++;
		i += 3;
 
		triangles[ i ] = tri - missTris + 1;
		triangles[ i+1 ] = tri - missTris + 2;
		triangles[ i+2 ] = tri - missTris + 0;
		
		tUsed[i] = 1;
		tUsed[i+1] = 1;
		tUsed[i+2] = 1;
		
		numUsed+=3;
		tri++;
		i += 3;
	}
	
    int[] trianglesA = new int[numUsed];
    
    jj = 0;
	for(int ii=0;ii<NT;ii++){
	    if(tUsed[ii] == 1){
			trianglesA[jj] = triangles[ii];
			gtriangles.Add(triangles[ii]+verticesOffset);
			jj++;
		}
	}
	
	jj = 0;
	minVertex.Add(verticesOffset);
	for(int ii=0;ii<NN;ii++){
	    
	    if(vUsed[ii] == 1){
			
			jj++;
		}
	}
	maxVertex.Add(verticesOffset+jj);
	verticesOffset = verticesOffset+jj;
	currentSegmentOffset = jj;
	
	minTriangle.Add(trianglesOffset);
	jj = 0;
	for(int ii=0;ii<NT;ii++){
	    if(tUsed[ii] == 1){
			
			jj++;
		}
	}
	//print(jj);
	maxTriangle.Add(trianglesOffset+jj);
	trianglesOffset = trianglesOffset+jj;
	
	currentSegmentId++;
	}
	
	GameObject CreateMesh(){
		GameObject plane = new GameObject("Tree");
		MeshFilter filter = plane.AddComponent<MeshFilter>();
		Mesh mesh = filter.mesh;
		mesh.Clear();
			
		mesh.vertices = gvertices.ToArray();
		mesh.normals = gnormales.ToArray();
		mesh.uv = guvs.ToArray();
		mesh.triangles = gtriangles.ToArray();
 
		mesh.RecalculateBounds();
		mesh.Optimize();
			
		MeshRenderer renderer = plane.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
		renderer.material.shader = Shader.Find ("Diffuse");
		Texture2D tex = new Texture2D(1, 50);
		tex.SetPixel(0, 0, Color.green);
		tex.Apply();
		renderer.material.mainTexture = tex;
		renderer.material.color = Color.green;

		return plane;
	}
}